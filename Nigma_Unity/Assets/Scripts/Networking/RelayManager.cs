using System;
using System.Threading.Tasks;
using UnityEngine;

// ─── Unity Gaming Services ──────────────────────────────────────────────────
// Requires:
//   - com.unity.services.relay
//   - com.unity.netcode.gameobjects
//   - com.unity.services.authentication (via NetworkBootstrapper)
// ────────────────────────────────────────────────────────────────────────────
#if UNITY_SERVICES_RELAY
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
#endif

namespace Nigma.Networking
{
    /// <summary>
    /// Encapsulates Unity Relay allocation logic.
    ///
    /// HOST path:
    ///   1. CreateAllocationAsync(maxPlayers) → gets an Allocation with a JoinCode
    ///   2. ConfigureHostTransport(allocation) → sets up UnityTransport for the host
    ///   3. Returns the JoinCode string (stored in LobbyManager as Room Code)
    ///
    /// CLIENT path:
    ///   1. JoinAllocationAsync(joinCode) → gets a JoinAllocation
    ///   2. ConfigureClientTransport(joinAllocation) → sets up UnityTransport for the client
    ///
    /// The Relay server acts as a rendezvous point; no port forwarding needed.
    /// </summary>
    public class RelayManager : MonoBehaviour
    {
        public static RelayManager Instance { get; private set; }

        [Header("Relay Settings")]
        [Tooltip("Maximum number of players per room (excluding host).")]
        public int maxConnections = 5; // Host + 5 clients = 6 total (matches design doc)

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        // ────────────────────────────────────────────────────────────────────
        #region Host — Create Relay Allocation

        /// <summary>
        /// Creates a Relay allocation and starts the NetworkManager as Host.
        /// Returns the Relay Join Code to pass to LobbyManager as the Room Code.
        /// </summary>
        public async Task<string> CreateRelayAndStartHostAsync()
        {
#if UNITY_SERVICES_RELAY
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                Debug.Log($"[RelayManager] Allocation created. Join code: {joinCode}");

                ConfigureHostTransport(allocation);
                NetworkManager.Singleton.StartHost();

                return joinCode;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RelayManager] Failed to create Relay allocation: {ex.Message}");
                return null;
            }
#else
            Debug.LogWarning("[RelayManager] Unity Relay package not installed.");
            await Task.CompletedTask;
            return null;
#endif
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Client — Join Relay Allocation

        /// <summary>
        /// Joins an existing Relay allocation using the Join Code and starts the
        /// NetworkManager as a client.
        /// </summary>
        public async Task<bool> JoinRelayAndStartClientAsync(string joinCode)
        {
#if UNITY_SERVICES_RELAY
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                Debug.Log($"[RelayManager] Joined allocation with code: {joinCode}");

                ConfigureClientTransport(joinAllocation);
                NetworkManager.Singleton.StartClient();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RelayManager] Failed to join Relay allocation: {ex.Message}");
                return false;
            }
#else
            Debug.LogWarning("[RelayManager] Unity Relay package not installed.");
            await Task.CompletedTask;
            return false;
#endif
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Transport Configuration

#if UNITY_SERVICES_RELAY
        private void ConfigureHostTransport(Allocation allocation)
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayData = new RelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.ConnectionData,
                allocation.ConnectionData,
                allocation.Key,
                false // isSecure: false for dtls-less; set true for dtls
            );
            transport.SetRelayServerData(relayData);
        }

        private void ConfigureClientTransport(JoinAllocation joinAllocation)
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayData = new RelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData,
                joinAllocation.Key,
                false
            );
            transport.SetRelayServerData(relayData);
        }
#endif

        #endregion
    }
}
