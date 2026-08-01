using System;
using System.Threading.Tasks;
using UnityEngine;

// ─── Unity Gaming Services ──────────────────────────────────────────────────
// Requires the following packages (install via Package Manager):
//   - com.unity.services.core
//   - com.unity.services.authentication
//
// NOTE: This script handles ANONYMOUS sign-in only. For production, consider
// linking accounts with Google Play / Game Center.
// ────────────────────────────────────────────────────────────────────────────
#if UNITY_SERVICES_CORE
using Unity.Services.Core;
using Unity.Services.Authentication;
#endif

namespace Nigma.Networking
{
    /// <summary>
    /// Bootstraps Unity Gaming Services (UGS) at app startup.
    /// Must be placed in the scene BEFORE LobbyManager.
    ///
    /// Flow:
    ///   1. UnityServices.InitializeAsync()
    ///   2. AuthenticationService.SignInAnonymouslyAsync()
    ///   3. Fires OnServicesReady — LobbyManager subscribes to this.
    ///
    /// If UGS packages are not installed, the script logs a warning and does nothing,
    /// so the project still compiles and can be tested offline.
    /// </summary>
    public class NetworkBootstrapper : MonoBehaviour
    {
        public static NetworkBootstrapper Instance { get; private set; }

        /// <summary>
        /// Fired when UGS is initialized and the player is signed in.
        /// Subscribe before the scene starts (use Awake in listeners).
        /// </summary>
        public static event Action OnServicesReady;

        public bool IsReady { get; private set; } = false;
        public string PlayerId { get; private set; } = string.Empty;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private async void Start()
        {
            await InitializeServicesAsync();
        }

        private async Task InitializeServicesAsync()
        {
#if UNITY_SERVICES_CORE
            try
            {
                // Initialize UGS
                await UnityServices.InitializeAsync();
                Debug.Log("[NetworkBootstrapper] Unity Services initialized.");

                // Sign in anonymously
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                PlayerId = AuthenticationService.Instance.PlayerId;
                IsReady  = true;

                Debug.Log($"[NetworkBootstrapper] Signed in anonymously. PlayerID: {PlayerId}");
                OnServicesReady?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[NetworkBootstrapper] Failed to initialize UGS: {ex.Message}\n" +
                               "Make sure you have configured a Project ID in Edit → Project Settings → Services.");
            }
#else
            Debug.LogWarning("[NetworkBootstrapper] Unity Services packages not installed. " +
                             "Multiplayer features will be unavailable. " +
                             "Install via Package Manager: com.unity.services.core + com.unity.services.authentication");
            await Task.CompletedTask;
#endif
        }
    }
}
