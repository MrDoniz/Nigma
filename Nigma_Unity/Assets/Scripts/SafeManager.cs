using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nigma.Core
{
    /// <summary>
    /// Gestiona la mecánica de la Caja Fuerte (Meta-puzzle de Escape Room).
    /// 
    /// Del GDD:
    ///  - De vez en cuando el juego ofrece una Herramienta legendaria encerrada en una caja fuerte virtual.
    ///  - La combinación se deduce del tablero activo (ej: "A - B = Código", donde A = personas en esquinas,
    ///    B = armas descubiertas).
    ///  - Al abrirla, el jugador recibe un Joker legendario que se añade a JokerManager.
    /// </summary>
    public class SafeManager : MonoBehaviour
    {
        public static SafeManager Instance;

        [Header("UI de la Caja Fuerte")]
        public GameObject safePanelRoot;     // Panel entero de la caja fuerte
        public InputField[] digitInputs;     // 4 campos de un solo dígito
        public Button confirmButton;
        public Text hintText;                // La pista de cómo calcular el código
        public Text resultText;             // "¡Abierta!" o "Código incorrecto"

        [Header("Recompensa Legendaria")]
        public JokerData legendaryJokerReward;

        private string secretCode = "";
        private bool isUnlocked = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (safePanelRoot != null) safePanelRoot.SetActive(false);
            if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClicked);
        }

        /// <summary>
        /// Abre la UI de la Caja Fuerte con un código y una pista específica para el nivel.
        /// Llamado por GameManager cuando el nivel lo requiere.
        /// </summary>
        public void OpenSafe(string code, string hint)
        {
            if (isUnlocked) return;

            secretCode = code;
            isUnlocked = false;

            if (hintText != null) hintText.text = $"🔒 Pista del candado:\n\"{hint}\"";
            if (resultText != null) resultText.text = "";
            if (safePanelRoot != null) safePanelRoot.SetActive(true);

            foreach (var input in digitInputs)
            {
                if (input != null) input.text = "";
            }
        }

        public void CloseSafe()
        {
            if (safePanelRoot != null) safePanelRoot.SetActive(false);
        }

        private void OnConfirmClicked()
        {
            string enteredCode = "";
            foreach (var input in digitInputs)
            {
                enteredCode += (input != null ? input.text : "0");
            }

            if (enteredCode == secretCode)
            {
                isUnlocked = true;
                if (resultText != null) resultText.text = "✅ ¡Caja Fuerte abierta! Has obtenido una Herramienta Legendaria.";

                // Conceder el Joker legendario
                if (legendaryJokerReward != null && JokerManager.Instance != null)
                {
                    JokerManager.Instance.activeJokers.Add(legendaryJokerReward);
                    Debug.Log($"[SafeManager] Joker legendario '{legendaryJokerReward.jokerName}' añadido!");
                }

                Invoke(nameof(CloseSafe), 3f);
            }
            else
            {
                if (resultText != null) resultText.text = "❌ Código incorrecto. Vuelve a examinar el tablero.";
                Debug.Log($"[SafeManager] Código incorrecto. Introducido: '{enteredCode}'. Esperado: '{secretCode}'.");
            }
        }
    }
}
