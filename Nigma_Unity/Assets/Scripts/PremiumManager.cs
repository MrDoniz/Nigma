using UnityEngine;
using System;

// ─────────────────────────────────────────────────────────────────────────────
// Para activar la integración REAL de Unity IAP:
//   1. En Unity Editor: Window → Package Manager → busca "In App Purchasing"
//      e instálalo (com.unity.purchasing).
//   2. Ve a Edit → Project Settings → Player → Other Settings → Scripting
//      Define Symbols, y añade: UNITY_PURCHASING
//   3. O usa la herramienta automática: Nigma → Herramientas → Configurar IAP
// ─────────────────────────────────────────────────────────────────────────────

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#endif

namespace Nigma.Core
{
    /// <summary>
    /// Gestiona las compras integradas (IAP) y el estado Premium del jugador.
    ///
    /// Modo Real (UNITY_PURCHASING definido):
    ///   Implementa IStoreListener completo para conectar con App Store / Google Play.
    ///
    /// Modo Mock (UNITY_PURCHASING NO definido):
    ///   Simula una compra exitosa tras 1 segundo. Útil para desarrollo en Editor.
    /// </summary>
    public class PremiumManager : MonoBehaviour
#if UNITY_PURCHASING
        , IStoreListener
#endif
    {
        public static PremiumManager Instance { get; private set; }

        // ── Constantes ───────────────────────────────────────────────────────
        private const string PREMIUM_KEY        = "Nigma_Premium_Unlocked";
        public  const string PREMIUM_PRODUCT_ID = "com.nigma.premium_unlock";

        // ── Eventos ──────────────────────────────────────────────────────────
        /// <summary>Se dispara cuando el estado Premium cambia. true = es Premium.</summary>
        public event Action<bool> OnPremiumStateChanged;

#if UNITY_PURCHASING
        private IStoreController   storeController;
        private IExtensionProvider extensionProvider;
        private bool               isInitialized => storeController != null;
#endif

        // ── Unity Lifecycle ──────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePurchasing();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ── Inicialización ───────────────────────────────────────────────────
        private void InitializePurchasing()
        {
#if UNITY_PURCHASING
            if (isInitialized) return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Producto no consumible: pago único de 4.99€
            builder.AddProduct(PREMIUM_PRODUCT_ID, ProductType.NonConsumable);

            UnityPurchasing.Initialize(this, builder);
            Debug.Log("[PremiumManager] Inicializando Unity IAP (modo REAL)...");
#else
            Debug.Log("[PremiumManager] Unity IAP no instalado. Usando modo MOCK.");
            Debug.Log("[PremiumManager] → Instala 'In App Purchasing' desde el Package Manager");
            Debug.Log("[PremiumManager] → Luego añade el símbolo UNITY_PURCHASING en Player Settings");
#endif
        }

        // ── API Pública ──────────────────────────────────────────────────────

        /// <returns>true si el jugador ha desbloqueado la versión Premium.</returns>
        public bool IsPremiumUnlocked()
        {
            return PlayerPrefs.GetInt(PREMIUM_KEY, 0) == 1;
        }

        /// <summary>Lanza el flujo de compra del producto Premium.</summary>
        public void BuyPremium()
        {
#if UNITY_PURCHASING
            if (!isInitialized)
            {
                Debug.LogWarning("[PremiumManager] La tienda no está inicializada todavía.");
                return;
            }
            Debug.Log($"[PremiumManager] Iniciando compra real: {PREMIUM_PRODUCT_ID}");
            storeController.InitiatePurchase(PREMIUM_PRODUCT_ID);
#else
            // MOCK: simula compra exitosa tras 1 segundo (para pruebas en Editor)
            Debug.Log($"[PremiumManager] [MOCK] Simulando compra de: {PREMIUM_PRODUCT_ID}");
            Invoke(nameof(MockPurchaseSuccess), 1f);
#endif
        }

        /// <summary>Restaura compras previas (necesario para iOS App Store).</summary>
        public void RestorePurchases()
        {
#if UNITY_PURCHASING && UNITY_IOS
            if (!isInitialized)
            {
                Debug.LogWarning("[PremiumManager] No se puede restaurar: tienda no inicializada.");
                return;
            }
            extensionProvider.GetExtension<IAppleExtensions>()
                .RestoreTransactions(result =>
                {
                    Debug.Log(result
                        ? "[PremiumManager] Restauración exitosa en iOS."
                        : "[PremiumManager] No se encontraron compras previas en iOS.");
                });
#elif UNITY_PURCHASING && UNITY_ANDROID
            // Google Play restaura automáticamente al inicializar la tienda.
            Debug.Log("[PremiumManager] Android: las compras se restauran automáticamente.");
#else
            // MOCK: comprueba PlayerPrefs
            if (IsPremiumUnlocked())
                Debug.Log("[PremiumManager] [MOCK] Restauración: ya eres Premium.");
            else
                Debug.Log("[PremiumManager] [MOCK] Restauración: no se encontraron compras previas.");
#endif
        }

        // ── Métodos Internos ─────────────────────────────────────────────────

        private void UnlockPremium()
        {
            PlayerPrefs.SetInt(PREMIUM_KEY, 1);
            PlayerPrefs.Save();
            Debug.Log("[PremiumManager] ¡Felicidades! Premium desbloqueado con éxito.");
            OnPremiumStateChanged?.Invoke(true);
        }

        /// <summary>Resetea el estado Premium (útil para testing en Editor).</summary>
        public void ResetPremiumState()
        {
            PlayerPrefs.DeleteKey(PREMIUM_KEY);
            PlayerPrefs.Save();
            Debug.Log("[PremiumManager] Estado Premium reseteado (ahora es Gratis).");
            OnPremiumStateChanged?.Invoke(false);
        }

#if !UNITY_PURCHASING
        // Mock solo activo cuando IAP no está instalado
        private void MockPurchaseSuccess() => UnlockPremium();
#endif

        // ── IStoreListener (Solo con UNITY_PURCHASING) ───────────────────────
#if UNITY_PURCHASING

        /// <summary>Callback: Unity IAP inicializado correctamente.</summary>
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController   = controller;
            extensionProvider = extensions;
            Debug.Log("[PremiumManager] Unity IAP inicializado correctamente.");

            // Si el producto ya está comprado (compra previa no restaurada en Android/iOS),
            // lo desbloqueamos automáticamente.
            Product product = storeController.products.WithID(PREMIUM_PRODUCT_ID);
            if (product != null && product.hasReceipt)
            {
                Debug.Log("[PremiumManager] Compra previa detectada. Desbloqueando Premium...");
                UnlockPremium();
            }
        }

        /// <summary>Callback: fallo al inicializar la tienda.</summary>
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"[PremiumManager] Error al inicializar IAP: {error}");
        }

        /// <summary>Callback: fallo al inicializar la tienda (con mensaje detallado).</summary>
        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"[PremiumManager] Error al inicializar IAP: {error} — {message}");
        }

        /// <summary>Callback: compra procesada. Aquí validamos el recibo.</summary>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            if (string.Equals(args.purchasedProduct.definition.id, PREMIUM_PRODUCT_ID,
                              StringComparison.Ordinal))
            {
                Debug.Log("[PremiumManager] Compra de Premium confirmada. Desbloqueando...");
                UnlockPremium();
            }
            else
            {
                Debug.LogWarning($"[PremiumManager] Producto desconocido: {args.purchasedProduct.definition.id}");
            }

            // Complete: la compra fue procesada con éxito.
            return PurchaseProcessingResult.Complete;
        }

        /// <summary>Callback: la compra falló.</summary>
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"[PremiumManager] Compra fallida de '{product.definition.storeSpecificId}': {failureReason}");
        }

#endif
    }
}
