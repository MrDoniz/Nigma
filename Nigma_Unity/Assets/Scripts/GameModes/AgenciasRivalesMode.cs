using System.Collections;
using UnityEngine;

// ─── Netcode for GameObjects ─────────────────────────────────────────────────
// Requires: com.unity.netcode.gameobjects
// ─────────────────────────────────────────────────────────────────────────────
#if UNITY_NETCODE
using Unity.Netcode;
#endif

namespace Nigma.GameModes
{
    /// <summary>
    /// Modo de juego: AGENCIAS RIVALES
    ///
    /// Mecánica: Carrera de velocidad sincrónica.
    ///   • Todos los jugadores reciben el mismo LevelData simultáneamente.
    ///   • Cada jugador trabaja en su propia copia LOCAL del tablero.
    ///   • Un timer global (sincronizado por el Host) cuenta el tiempo.
    ///   • Al acertar "Resolver", el jugador envía un ServerRpc notificando victoria.
    ///   • El servidor calcula el ranking y lo broadcast a todos.
    ///   • Penalización: respuesta incorrecta → +30 segundos al tiempo del jugador.
    ///
    /// Arquitectura de red:
    ///   • HOST controla el timer y valida las victorias.
    ///   • CLIENTS envían ServerRpc al pulsar "Resolver".
    ///   • HOST hace ClientRpc para broadcast del resultado.
    /// </summary>
#if UNITY_NETCODE
    public class AgenciasRivalesMode : NetworkBehaviour
#else
    public class AgenciasRivalesMode : MonoBehaviour
#endif
    {
        [Header("Mode Settings")]
        [Tooltip("Penalty time in seconds added for each incorrect answer.")]
        public float incorrectAnswerPenalty = 30f;
        [Tooltip("Maximum time for the race in seconds (0 = unlimited).")]
        public float maxRaceTime = 300f; // 5 minutes default

        // ── State (all clients) ──────────────────────────────────────────────
        private float localTime   = 0f;    // Each client tracks their own elapsed time
        private bool  raceActive  = false;
        private bool  hasFinished = false;

        // ── State (Host only) ────────────────────────────────────────────────
        private int finishedPlayersCount = 0;
        private int totalPlayers = 0;

        // ─── Ranking ─────────────────────────────────────────────────────────
        // RankEntry[0] = 1st place, etc.
        [System.Serializable]
        public struct RankEntry
        {
            public ulong clientId;
            public float completionTime;
            public int   place;
        }

        private System.Collections.Generic.List<RankEntry> ranking =
            new System.Collections.Generic.List<RankEntry>();

        // ────────────────────────────────────────────────────────────────────
        #region Unity Lifecycle

        private void Update()
        {
            if (raceActive && !hasFinished)
            {
                localTime += Time.deltaTime;

                if (maxRaceTime > 0 && localTime >= maxRaceTime)
                {
                    OnTimeOut();
                }
            }
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Public API (called by GameManager)

        /// <summary>
        /// Called by the Host's GameManager to start the race for all players.
        /// Loads the level data and signals all clients to begin.
        /// </summary>
        public void StartRace(int levelID, int playerCount)
        {
#if UNITY_NETCODE
            if (!IsHost)
            {
                Debug.LogWarning("[AgenciasRivales] Only host can start the race.");
                return;
            }
#endif
            totalPlayers = playerCount;
            finishedPlayersCount = 0;
            ranking.Clear();

            Debug.Log($"[AgenciasRivales] Race starting! Level {levelID} | {playerCount} players.");

#if UNITY_NETCODE
            StartRaceClientRpc(levelID);
#else
            // Offline simulation
            OnRaceStarted(levelID);
#endif
        }

        /// <summary>
        /// Called by GameManager when the LOCAL player clicks "Resolver" and the answer
        /// is correct. Sends the result to the Host.
        /// </summary>
        public void SubmitCorrectAnswer()
        {
            if (hasFinished) return;
            hasFinished = true;
            raceActive  = false;

            float finalTime = localTime;
            Debug.Log($"[AgenciasRivales] Answer submitted. Time: {finalTime:F1}s");

#if UNITY_NETCODE
            NotifyVictoryServerRpc(finalTime);
#else
            // Offline simulation
            Debug.Log($"[AgenciasRivales] (Offline) Race complete in {finalTime:F1}s! 1st place.");
#endif
        }

        /// <summary>
        /// Called by GameManager when the LOCAL player answers incorrectly.
        /// Applies the time penalty.
        /// </summary>
        public void SubmitIncorrectAnswer()
        {
            if (hasFinished) return;
            localTime += incorrectAnswerPenalty;
            Debug.Log($"[AgenciasRivales] Wrong answer! +{incorrectAnswerPenalty}s penalty. Total: {localTime:F1}s");
        }

        /// <summary>
        /// Returns the current elapsed time for the LOCAL player (for UI display).
        /// </summary>
        public float GetLocalTime() => localTime;

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Network RPCs

#if UNITY_NETCODE

        /// <summary>
        /// HOST → ALL CLIENTS: Start the race with the specified level.
        /// </summary>
        [ClientRpc]
        private void StartRaceClientRpc(int levelID)
        {
            OnRaceStarted(levelID);
        }

        /// <summary>
        /// CLIENT → HOST: Notify host that this client finished with a given time.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void NotifyVictoryServerRpc(float completionTime, ServerRpcParams rpcParams = default)
        {
            ulong senderId = rpcParams.Receive.SenderClientId;
            finishedPlayersCount++;

            ranking.Add(new RankEntry
            {
                clientId       = senderId,
                completionTime = completionTime,
                place          = finishedPlayersCount
            });

            Debug.Log($"[AgenciasRivales] Player {senderId} finished in place {finishedPlayersCount} ({completionTime:F1}s)");

            // Notify ALL clients of the updated ranking
            BroadcastRankingClientRpc(senderId, completionTime, finishedPlayersCount);

            // If everyone is done, show final results
            if (finishedPlayersCount >= totalPlayers)
            {
                ShowFinalResultsClientRpc();
            }
        }

        /// <summary>
        /// HOST → ALL CLIENTS: A player placed in the ranking.
        /// </summary>
        [ClientRpc]
        private void BroadcastRankingClientRpc(ulong finishedClientId, float time, int place)
        {
            string medal = place == 1 ? "🥇" : place == 2 ? "🥈" : place == 3 ? "🥉" : $"#{place}";
            Debug.Log($"[AgenciasRivales] Player {finishedClientId} → {medal} ({time:F1}s)");

            // TODO (Phase 5): Update the ranking UI panel with this entry
        }

        /// <summary>
        /// HOST → ALL CLIENTS: All players have finished. Show final scoreboard.
        /// </summary>
        [ClientRpc]
        private void ShowFinalResultsClientRpc()
        {
            Debug.Log("[AgenciasRivales] Race over! All players finished.");
            // TODO (Phase 5): Load results scene or show Results Panel via UIManager
        }

#endif

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Private Helpers

        private void OnRaceStarted(int levelID)
        {
            localTime   = 0f;
            raceActive  = true;
            hasFinished = false;

            Debug.Log($"[AgenciasRivales] Race started for Level {levelID}!");

            // Signal GameManager to load the correct level locally
            // (GameManager listens to this via event or direct call)
            Nigma.Core.GameManager.Instance?.LoadLevel(levelID);
        }

        private void OnTimeOut()
        {
            raceActive  = false;
            hasFinished = true;
            Debug.Log("[AgenciasRivales] Time ran out!");

#if UNITY_NETCODE
            // Submit with max time (effectively last place)
            NotifyVictoryServerRpc(maxRaceTime + incorrectAnswerPenalty);
#endif
        }

        #endregion
    }
}
