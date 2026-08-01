using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// ─── Unity Gaming Services ──────────────────────────────────────────────────
// Requires:
//   - com.unity.services.lobby
//   - com.unity.services.relay  (via RelayManager)
//   - com.unity.services.authentication (via NetworkBootstrapper)
// ────────────────────────────────────────────────────────────────────────────
#if UNITY_SERVICES_LOBBY
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
#endif

namespace Nigma.Networking
{
    /// <summary>
    /// Manages the full lifecycle of a Nigma multiplayer room:
    ///
    ///   HOST:
    ///     CreateRoom() → generates a 4-letter Room Code → creates a Unity Lobby
    ///                 → allocates Relay → starts as Host → sends heartbeat
    ///
    ///   CLIENT:
    ///     JoinRoom(code) → queries Lobby by Room Code → retrieves Relay join code
    ///                   → joins Relay → starts as Client
    ///
    /// Room Code format: 4 uppercase letters, excluding I, O (disambiguation).
    /// </summary>
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }

        // ── Constants ───────────────────────────────────────────────────────
        private const int    ROOM_CODE_LENGTH     = 4;
        private const string ROOM_CODE_KEY        = "RoomCode";       // Lobby data key — queried by clients
        private const string RELAY_CODE_KEY       = "RelayJoinCode";  // Lobby data key
        private const string GAME_MODE_KEY        = "GameMode";       // Lobby data key
        private const float  HEARTBEAT_INTERVAL   = 15f;             // Unity Lobby expires after 30s
        private const int    MAX_PLAYERS          = 6;               // Host + 5 clients

        // ── Disambiguation-safe alphabet (no I, O, 0, 1) ───────────────────
        private const string ROOM_CODE_ALPHABET = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        // ── State ───────────────────────────────────────────────────────────
        public bool   IsHost          { get; private set; } = false;
        public string RoomCode        { get; private set; } = string.Empty;
        public int    ConnectedPlayers { get; private set; } = 0;
        public string SelectedGameMode { get; private set; } = "AgenciasRivales";

        public event Action<string> OnRoomCreated;    // (roomCode)
        public event Action         OnRoomJoined;
        public event Action<int>    OnPlayerCountChanged;   // (count)
        public event Action<string> OnRoomError;      // (errorMessage)

#if UNITY_SERVICES_LOBBY
        private Lobby currentLobby;
#endif
        private Coroutine heartbeatCoroutine;
        private Coroutine pollCoroutine;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void OnEnable()
        {
            NetworkBootstrapper.OnServicesReady += OnServicesReady;
        }

        private void OnDisable()
        {
            NetworkBootstrapper.OnServicesReady -= OnServicesReady;
        }

        private void OnServicesReady()
        {
            Debug.Log("[LobbyManager] UGS ready. Lobby operations enabled.");
        }

        // ────────────────────────────────────────────────────────────────────
        #region Public API — Host

        /// <summary>
        /// Host flow: Create a Relay allocation, create a Unity Lobby with the
        /// generated Room Code, and start NetworkManager as Host.
        /// </summary>
        public async Task CreateRoomAsync(string gameMode = "AgenciasRivales")
        {
            if (!NetworkBootstrapper.Instance.IsReady)
            {
                OnRoomError?.Invoke("Servicios de red no disponibles. Comprueba tu conexión.");
                return;
            }

            SelectedGameMode = gameMode;

#if UNITY_SERVICES_LOBBY
            try
            {
                // 1. Generate Room Code
                RoomCode = GenerateRoomCode();
                IsHost = true;

                // 2. Create Relay allocation and get join code
                string relayJoinCode = await RelayManager.Instance.CreateRelayAndStartHostAsync();
                if (relayJoinCode == null)
                {
                    OnRoomError?.Invoke("No se pudo crear la sala de Relay.");
                    return;
                }

                // 3. Create Unity Lobby with public Room Code and private Relay code
                var options = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Data = new Dictionary<string, DataObject>
                    {
                        // Public: can be queried by Room Code
                        [ROOM_CODE_KEY] = new DataObject(DataObject.VisibilityOptions.Public, RoomCode),
                        // Public: clients need this to join Relay
                        [RELAY_CODE_KEY] = new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode),
                        // Public: game mode selection
                        [GAME_MODE_KEY] = new DataObject(DataObject.VisibilityOptions.Public, gameMode)
                    }
                };

                currentLobby = await LobbyService.Instance.CreateLobbyAsync(
                    lobbyName: $"Nigma-{RoomCode}",
                    maxPlayers: MAX_PLAYERS,
                    options: options
                );

                Debug.Log($"[LobbyManager] Room created. Code: {RoomCode} | LobbyID: {currentLobby.Id}");

                // 4. Start heartbeat to keep lobby alive
                heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine());
                // 5. Poll lobby for player count updates
                pollCoroutine = StartCoroutine(PollLobbyCoroutine());

                ConnectedPlayers = currentLobby.Players.Count;
                OnRoomCreated?.Invoke(RoomCode);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LobbyManager] CreateRoom failed: {ex.Message}");
                OnRoomError?.Invoke($"Error al crear sala: {ex.Message}");
            }
#else
            // Offline simulation for testing without UGS packages
            RoomCode = GenerateRoomCode();
            IsHost = true;
            ConnectedPlayers = 1;
            OnRoomCreated?.Invoke(RoomCode);
            Debug.LogWarning("[LobbyManager] Running in offline mode — Unity Lobby package not installed.");
            await Task.CompletedTask;
#endif
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Public API — Client

        /// <summary>
        /// Client flow: Query Unity Lobby by Room Code, retrieve Relay join code,
        /// join Relay, and start NetworkManager as Client.
        /// </summary>
        public async Task JoinRoomAsync(string roomCode)
        {
            if (!NetworkBootstrapper.Instance.IsReady)
            {
                OnRoomError?.Invoke("Servicios de red no disponibles.");
                return;
            }

#if UNITY_SERVICES_LOBBY
            try
            {
                // 1. Query lobby by Room Code
                var queryOptions = new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(
                            field: QueryFilter.FieldOptions.S1,  // Custom field slot
                            op: QueryFilter.OpOptions.EQ,
                            value: roomCode.ToUpper()
                        )
                    }
                };

                QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);

                if (response.Results.Count == 0)
                {
                    OnRoomError?.Invoke($"No se encontró ninguna sala con el código '{roomCode}'.");
                    return;
                }

                currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(response.Results[0].Id);

                // 2. Retrieve Relay join code from lobby data
                string relayCode = currentLobby.Data[RELAY_CODE_KEY].Value;
                RoomCode = roomCode.ToUpper();
                IsHost = false;

                // 3. Join Relay and start as Client
                bool joined = await RelayManager.Instance.JoinRelayAndStartClientAsync(relayCode);
                if (!joined)
                {
                    OnRoomError?.Invoke("No se pudo conectar al servidor de la sala.");
                    return;
                }

                // 4. Read game mode
                SelectedGameMode = currentLobby.Data[GAME_MODE_KEY].Value;
                ConnectedPlayers = currentLobby.Players.Count;

                Debug.Log($"[LobbyManager] Joined room {RoomCode} | Mode: {SelectedGameMode}");
                OnRoomJoined?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LobbyManager] JoinRoom failed: {ex.Message}");
                OnRoomError?.Invoke($"Error al unirse: {ex.Message}");
            }
#else
            Debug.LogWarning("[LobbyManager] Running in offline mode — Unity Lobby package not installed.");
            RoomCode = roomCode.ToUpper();
            ConnectedPlayers = 2;
            OnRoomJoined?.Invoke();
            await Task.CompletedTask;
#endif
        }

        /// <summary>
        /// Called by Host to change the game mode selection before starting.
        /// </summary>
        public void SetGameMode(string gameMode)
        {
            SelectedGameMode = gameMode;
            Debug.Log($"[LobbyManager] Game mode set to: {gameMode}");
        }

        /// <summary>
        /// Called by Host to kick off the game. Triggers an RPC that loads the
        /// game scene for all clients. Called from LobbyUIController.
        /// </summary>
        public void StartGame()
        {
            if (!IsHost)
            {
                Debug.LogWarning("[LobbyManager] Only the host can start the game.");
                return;
            }
            if (ConnectedPlayers < 2)
            {
                OnRoomError?.Invoke("Se necesitan al menos 2 jugadores para empezar.");
                return;
            }

            Debug.Log($"[LobbyManager] Starting game in mode: {SelectedGameMode}");
            // The actual scene load RPC is handled by AgenciasRivalesMode / PoliciaCorruptoMode
        }

        /// <summary>
        /// Leaves the current lobby and disconnects. Called on scene change or app exit.
        /// </summary>
        public async Task LeaveRoomAsync()
        {
#if UNITY_SERVICES_LOBBY
            if (currentLobby == null) return;

            if (heartbeatCoroutine != null) StopCoroutine(heartbeatCoroutine);
            if (pollCoroutine != null) StopCoroutine(pollCoroutine);

            try
            {
                if (IsHost)
                {
                    await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                    Debug.Log("[LobbyManager] Lobby deleted by host.");
                }
                else
                {
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, NetworkBootstrapper.Instance.PlayerId);
                    Debug.Log("[LobbyManager] Client left lobby.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[LobbyManager] LeaveRoom cleanup error (safe to ignore): {ex.Message}");
            }

            currentLobby = null;
#else
            await Task.CompletedTask;
#endif
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Heartbeat & Polling

#if UNITY_SERVICES_LOBBY
        private IEnumerator HeartbeatCoroutine()
        {
            while (currentLobby != null && IsHost)
            {
                yield return new WaitForSeconds(HEARTBEAT_INTERVAL);
                if (currentLobby != null)
                {
                    LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
                }
            }
        }

        private IEnumerator PollLobbyCoroutine()
        {
            while (currentLobby != null)
            {
                yield return new WaitForSeconds(3f); // Poll every 3s
                try
                {
                    // Note: Polling is async; we fire-and-forget here for simplicity.
                    // A production version would use lobby callbacks instead.
                    UpdateLobbyAsync();
                }
                catch { /* Ignore poll errors */ }
            }
        }

        private async void UpdateLobbyAsync()
        {
            if (currentLobby == null) return;
            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
            int newCount = currentLobby.Players.Count;
            if (newCount != ConnectedPlayers)
            {
                ConnectedPlayers = newCount;
                OnPlayerCountChanged?.Invoke(ConnectedPlayers);
            }
        }
#endif

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Room Code Generation

        /// <summary>
        /// Generates a random 4-character Room Code from ROOM_CODE_ALPHABET.
        /// Avoids I, O, 0, 1 to prevent confusion when reading aloud.
        /// </summary>
        private string GenerateRoomCode()
        {
            char[] code = new char[ROOM_CODE_LENGTH];
            for (int i = 0; i < ROOM_CODE_LENGTH; i++)
            {
                code[i] = ROOM_CODE_ALPHABET[UnityEngine.Random.Range(0, ROOM_CODE_ALPHABET.Length)];
            }
            return new string(code);
        }

        #endregion
    }
}
