using System.Collections.Generic;
using UnityEngine;
using Nigma.Networking;

// ─── Netcode for GameObjects ─────────────────────────────────────────────────
// Requires: com.unity.netcode.gameobjects
// ─────────────────────────────────────────────────────────────────────────────
#if UNITY_NETCODE
using Unity.Netcode;
#endif

namespace Nigma.GameModes
{
    /// <summary>
    /// Modo de juego: POLICÍA CORRUPTO
    ///
    /// Mecánica: Distribución de pistas asimétrica (Party Game con roles ocultos).
    ///   • El HOST carga un LevelData con lista de pistas fragmentadas (multiplayerClueFragments).
    ///   • El servidor distribuye pistas entre los detectives de forma que cada uno
    ///     recibe solo ALGUNAS pistas — nadie tiene la imagen completa solo.
    ///   • A UN jugador aleatorio se le asigna el rol de "El Corrupto" en secreto.
    ///     El Corrupto recibe 1-2 pistas falsas (contradictorias) en lugar de las reales.
    ///   • Los jugadores hablan en voz alta para compartir pistas y llegar a la solución.
    ///   • Para ganar: los detectives deben acertar la solución Y votar correctamente
    ///     quién es el Corrupto. Si el Corrupto pasa desapercibido, él gana.
    ///
    /// Nota de diseño: el juego NO impone chat de texto ni de voz.
    ///   Se juega en persona o con una app de voz externa (Discord, etc.).
    ///
    /// Fragmentación de pistas:
    ///   - Las pistas se leen de LevelData.multiplayerClueFragments.
    ///   - Si la lista está vacía, el puzzleDescription se parte por saltos de línea.
    ///   - El Corrupto recibe las pistas normales pero 1 o 2 son sustituidas por
    ///     versiones invertidas/falsas generadas proceduralmente.
    /// </summary>
#if UNITY_NETCODE
    public class PoliciaCorruptoMode : NetworkBehaviour
#else
    public class PoliciaCorruptoMode : MonoBehaviour
#endif
    {
        // ── State (Host only) ────────────────────────────────────────────────
        private ulong corruptoClientId;
        private bool  corruptoRevealed = false;
        private int   votesSubmitted   = 0;
        private Dictionary<ulong, ulong> votes = new Dictionary<ulong, ulong>(); // voter → accused

        // ── State (all clients) ──────────────────────────────────────────────
        public bool IsCorrupto { get; private set; } = false;
        public List<string> MyClues { get; private set; } = new List<string>();

        // ── Current Level ────────────────────────────────────────────────────
        private Nigma.Core.LevelData currentLevel;

        // ────────────────────────────────────────────────────────────────────
        #region Public API (called by LobbyManager / GameManager)

        /// <summary>
        /// HOST: Starts the Policía Corrupto session.
        ///   1. Distributes clue fragments to each connected client.
        ///   2. Randomly assigns the Corrupto role.
        ///   3. Sends corrupted clues to the Corrupto client.
        /// </summary>
        public void StartSession(Nigma.Core.LevelData levelData, List<ulong> connectedClientIds)
        {
#if UNITY_NETCODE
            if (!IsHost) return;
#endif
            currentLevel = levelData;

            // Build clue pool
            List<string> cluePool = BuildCluePool(levelData);

            if (cluePool.Count == 0)
            {
                Debug.LogWarning("[PoliciaCorrupto] No clue fragments found in LevelData. " +
                                 "Add entries to multiplayerClueFragments or ensure puzzleDescription has line breaks.");
                return;
            }

            // Assign Corrupto randomly
            int corruptoIndex = Random.Range(0, connectedClientIds.Count);
            corruptoClientId = connectedClientIds[corruptoIndex];
            corruptoRevealed = false;
            votes.Clear();
            votesSubmitted = 0;

            Debug.Log($"[PoliciaCorrupto] El Corrupto es el jugador con ID: {corruptoClientId} (secreto)");

            // Distribute clues
            for (int i = 0; i < connectedClientIds.Count; i++)
            {
                ulong clientId = connectedClientIds[i];
                bool isThisClientCorrupto = (clientId == corruptoClientId);
                List<string> clientClues = DistributeClues(cluePool, i, connectedClientIds.Count, isThisClientCorrupto);

                // Send clues to this client
#if UNITY_NETCODE
                SendCluesClientRpc(
                    string.Join("|CLUE|", clientClues),
                    isThisClientCorrupto,
                    new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } } }
                );
#else
                // Offline: just assign to local
                MyClues = clientClues;
                IsCorrupto = isThisClientCorrupto;
                Debug.Log($"[PoliciaCorrupto] (Offline) Clues assigned. Is Corrupto: {IsCorrupto}");
                foreach (string clue in MyClues) Debug.Log($"  → Pista: {clue}");
#endif
            }
        }

        /// <summary>
        /// CLIENT: Submits this player's vote for who they think is the Corrupto.
        /// </summary>
        public void SubmitVote(ulong accusedClientId)
        {
#if UNITY_NETCODE
            SubmitVoteServerRpc(accusedClientId);
#else
            // Offline simulation
            Debug.Log($"[PoliciaCorrupto] (Offline) Vote submitted against player {accusedClientId}.");
            if (accusedClientId == corruptoClientId)
                Debug.Log("[PoliciaCorrupto] (Offline) Detectives WIN — Corrupto identified!");
            else
                Debug.Log("[PoliciaCorrupto] (Offline) El Corrupto WINS — passed undetected!");
#endif
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Network RPCs

#if UNITY_NETCODE

        /// <summary>
        /// HOST → SPECIFIC CLIENT: Sends this client's private clue list and role.
        /// </summary>
        [ClientRpc]
        private void SendCluesClientRpc(string cluesJoined, bool isCorrupto, ClientRpcParams rpcParams = default)
        {
            MyClues.Clear();
            MyClues.AddRange(cluesJoined.Split(new[] { "|CLUE|" }, System.StringSplitOptions.RemoveEmptyEntries));
            IsCorrupto = isCorrupto;

            Debug.Log($"[PoliciaCorrupto] Clues received. Is Corrupto: {IsCorrupto}");
            foreach (string clue in MyClues)
                Debug.Log($"  → Pista: {clue}");

            // TODO (Phase 5): Update UIManager to display MyClues in the Atestado panel
        }

        /// <summary>
        /// CLIENT → HOST: Submit vote for who this client thinks is the Corrupto.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void SubmitVoteServerRpc(ulong accusedClientId, ServerRpcParams rpcParams = default)
        {
            ulong voterId = rpcParams.Receive.SenderClientId;
            if (votes.ContainsKey(voterId)) return; // Already voted

            votes[voterId] = accusedClientId;
            votesSubmitted++;

            // Tally when all votes are in
            // (totalPlayers tracked by LobbyManager)
            int totalPlayers = LobbyManager.Instance.ConnectedPlayers;
            if (votesSubmitted >= totalPlayers)
            {
                ResolveVotes();
            }
        }

#endif

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Vote Resolution (Host only)

        private void ResolveVotes()
        {
            // Count votes per accused player
            Dictionary<ulong, int> voteCount = new Dictionary<ulong, int>();
            foreach (var kvp in votes)
            {
                if (!voteCount.ContainsKey(kvp.Value)) voteCount[kvp.Value] = 0;
                voteCount[kvp.Value]++;
            }

            // Find most accused
            ulong mostAccused = 0;
            int maxVotes = 0;
            foreach (var kvp in voteCount)
            {
                if (kvp.Value > maxVotes)
                {
                    maxVotes = kvp.Value;
                    mostAccused = kvp.Key;
                }
            }

            bool detectivesWin = (mostAccused == corruptoClientId);

            Debug.Log(detectivesWin
                ? $"[PoliciaCorrupto] Detectives WIN! El Corrupto era el jugador {corruptoClientId}."
                : $"[PoliciaCorrupto] El Corrupto WINS! El acusado fue {mostAccused}, pero el Corrupto era {corruptoClientId}.");

#if UNITY_NETCODE
            RevealResultClientRpc(corruptoClientId, detectivesWin);
#endif
        }

#if UNITY_NETCODE
        /// <summary>
        /// HOST → ALL CLIENTS: Reveal the Corrupto identity and final result.
        /// </summary>
        [ClientRpc]
        private void RevealResultClientRpc(ulong corruptoId, bool detectivesWin)
        {
            corruptoRevealed = true;
            string result = detectivesWin
                ? "🕵️ ¡Detectives ganan! El Corrupto ha sido desenmascarado."
                : "🎭 ¡El Corrupto gana! Nadie lo descubrió.";

            Debug.Log($"[PoliciaCorrupto] RESULTADO: {result}");
            Debug.Log($"[PoliciaCorrupto] El Corrupto era el jugador {corruptoId}.");

            // TODO (Phase 5): Show reveal animation + result panel via UIManager
        }
#endif

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Clue Distribution Logic

        /// <summary>
        /// Builds the clue pool from LevelData.
        /// Priority: multiplayerClueFragments list → split puzzleDescription by line.
        /// </summary>
        private List<string> BuildCluePool(Nigma.Core.LevelData levelData)
        {
            var pool = new List<string>();

            if (levelData.multiplayerClueFragments != null && levelData.multiplayerClueFragments.Count > 0)
            {
                pool.AddRange(levelData.multiplayerClueFragments);
            }
            else if (!string.IsNullOrEmpty(levelData.puzzleDescription))
            {
                // Split by line — each sentence/line becomes one clue fragment
                string[] lines = levelData.puzzleDescription.Split('\n');
                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (trimmed.Length > 0) pool.Add(trimmed);
                }
            }

            return pool;
        }

        /// <summary>
        /// Distributes clues from the pool to a single player.
        /// Each player receives roughly (pool.Count / playerCount) clues, rotated
        /// so everyone gets different ones. The Corrupto's clues may be corrupted.
        /// </summary>
        private List<string> DistributeClues(List<string> pool, int playerIndex, int totalPlayers, bool corruptThis)
        {
            var playerClues = new List<string>();
            int cluesPerPlayer = Mathf.Max(1, Mathf.CeilToInt((float)pool.Count / totalPlayers));

            // Each player gets clues starting at their index offset (round-robin style)
            for (int i = 0; i < cluesPerPlayer; i++)
            {
                int clueIndex = (playerIndex + i * totalPlayers) % pool.Count;
                playerClues.Add(pool[clueIndex]);
            }

            // If this is the Corrupto, corrupt 1-2 clues
            if (corruptThis && playerClues.Count > 0)
            {
                int clueToCorrupt = Random.Range(0, playerClues.Count);
                playerClues[clueToCorrupt] = CorruptClue(playerClues[clueToCorrupt]);

                // Corrupt a second one if there are enough clues
                if (playerClues.Count > 2)
                {
                    int secondClue = (clueToCorrupt + 1) % playerClues.Count;
                    playerClues[secondClue] = CorruptClue(playerClues[secondClue]);
                }
            }

            return playerClues;
        }

        /// <summary>
        /// Generates a "corrupted" (false) version of a clue.
        /// Simple heuristic: prepends a negation marker. In a full implementation,
        /// each LevelData would have explicit "false clue" alternatives per fragment.
        ///
        /// TODO (Phase 5): Add a List falseClueFragments to LevelData for handcrafted
        ///                 false clues per fragment for narrative quality.
        /// </summary>
        private string CorruptClue(string originalClue)
        {
            // Heuristic corruptions — good enough for prototype; replace with handcrafted in Phase 5
            string[] corruptions = {
                $"[NOTA INTERNA: Dato incorrecto en el expediente] {originalClue}",
                $"Contrariamente a lo registrado: {originalClue.ToLower()}",
                $"Archivo corrupto: información no verificable.",
            };
            return corruptions[Random.Range(0, corruptions.Length)];
        }

        #endregion
    }
}
