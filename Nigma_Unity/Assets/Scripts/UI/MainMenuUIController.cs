using UnityEngine;
using UnityEngine.UI;

namespace Nigma.UI
{
    /// <summary>
    /// Controlador principal para el verdadero Menú del Juego (GDD).
    /// </summary>
    public class MainMenuUIController : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject panelMainMenu;
        public GameObject panelMultiplayerLobby; // El antiguo panelMain del LobbyUIController

        [Header("Main Menu Buttons")]
        public Button btnDailyCase;
        public Button btnCampaign;
        public Button btnEndless;
        public Button btnMultiplayer;
        public Button btnAgency;

        private void Start()
        {
            ShowMainMenu();

            if (btnMultiplayer != null)
                btnMultiplayer.onClick.AddListener(ShowMultiplayerLobby);
        }

        public void ShowMainMenu()
        {
            if (panelMainMenu != null) panelMainMenu.SetActive(true);
            if (panelMultiplayerLobby != null) panelMultiplayerLobby.SetActive(false);
        }

        public void ShowMultiplayerLobby()
        {
            if (panelMainMenu != null) panelMainMenu.SetActive(false);
            if (panelMultiplayerLobby != null) panelMultiplayerLobby.SetActive(true);
        }
    }
}
