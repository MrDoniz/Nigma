using UnityEngine;
using UnityEngine.UI;

namespace Nigma.Core
{
    /// <summary>
    /// Gestiona toda la UI del juego:
    ///   - Atestado Policial (texto del enigma)
    ///   - Maletín (inventario de muebles del nivel, Fase 2-4)
    ///   - Paneles de Victoria / Derrota
    ///   - Feedback textual al jugador
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Atestado (Riddle) UI")]
        public Text puzzleTextUI;
        public Text feedbackTextUI;

        [Header("Maletín (Inventory) UI — Fase 2/3")]
        public Text mirrorsCountText;
        public Text sofasCountText;

        [Header("Maletín (Inventory) UI — Fase 4")]
        public Text camerasCountText;
        public Text lampsCountText;
        public Text plantsCountText;
        public Text fansCountText;

        [Header("Panels")]
        public GameObject victoryPanel;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        // ────────────────────────────────────────────────────────────────────
        #region Atestado

        public void UpdatePuzzleText(string text)
        {
            if (puzzleTextUI != null) puzzleTextUI.text = text;
        }

        public void UpdateFeedbackText(string text)
        {
            if (feedbackTextUI != null) feedbackTextUI.text = text;
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Maletín (Inventory)

        /// <summary>
        /// Actualiza el contador de inventario para los muebles de Fase 2/3.
        /// Llamado por GameManager al cargar un nivel.
        /// </summary>
        public void UpdateInventory(int mirrors, int sofas)
        {
            SetCounterText(mirrorsCountText, "Espejos", mirrors);
            SetCounterText(sofasCountText,   "Sofás",   sofas);
        }

        /// <summary>
        /// Actualiza el inventario completo incluyendo los objetos de Fase 4.
        /// Llamado por GameManager cuando el LevelData tiene campos de Fase 4.
        /// </summary>
        public void UpdateInventoryFull(int mirrors, int sofas, int cameras, int lamps, int plants, int fans)
        {
            SetCounterText(mirrorsCountText, "Espejos",  mirrors);
            SetCounterText(sofasCountText,   "Sofás",    sofas);
            SetCounterText(camerasCountText, "Cámaras",  cameras);
            SetCounterText(lampsCountText,   "Lámparas", lamps);
            SetCounterText(plantsCountText,  "Plantas",  plants);
            SetCounterText(fansCountText,    "Ventiladores", fans);
        }

        /// <summary>
        /// Decrement a specific inventory counter by 1 (e.g., when player places a piece).
        /// </summary>
        public void DecrementCounter(FurnitureType type)
        {
            switch (type)
            {
                case FurnitureType.Mirror:  UpdateTextByDelta(mirrorsCountText, -1); break;
                case FurnitureType.Sofa:    UpdateTextByDelta(sofasCountText,   -1); break;
                case FurnitureType.Camera:  UpdateTextByDelta(camerasCountText, -1); break;
                case FurnitureType.Lamp:    UpdateTextByDelta(lampsCountText,   -1); break;
                case FurnitureType.Plant:   UpdateTextByDelta(plantsCountText,  -1); break;
                case FurnitureType.Fan:     UpdateTextByDelta(fansCountText,    -1); break;
            }
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Panels

        public void ShowVictoryPanel(bool show)
        {
            if (victoryPanel != null) victoryPanel.SetActive(show);
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Helpers

        private void SetCounterText(Text textUI, string label, int count)
        {
            if (textUI == null) return;
            // Hide the counter entirely if count is 0 (keeps the UI clean)
            textUI.gameObject.SetActive(count > 0);
            textUI.text = $"{label}: {count}";
        }

        private void UpdateTextByDelta(Text textUI, int delta)
        {
            if (textUI == null) return;

            // Parse current number from text (format "Label: N")
            string raw = textUI.text;
            int colonIdx = raw.LastIndexOf(':');
            if (colonIdx >= 0 && int.TryParse(raw.Substring(colonIdx + 1).Trim(), out int current))
            {
                int newVal = Mathf.Max(0, current + delta);
                textUI.text = raw.Substring(0, colonIdx + 1) + " " + newVal;
                textUI.gameObject.SetActive(newVal > 0);
            }
        }

        #endregion
    }
}
