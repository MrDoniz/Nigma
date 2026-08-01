using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nigma.Networking
{
    /// <summary>
    /// Controla la pantalla de Lobby en la escena del menú principal.
    ///
    /// Estructura de Canvas esperada:
    ///   LobbyCanvas
    ///     ├── Panel_Main          (botones Host / Join)
    ///     ├── Panel_Host          (código de sala + lista de jugadores + selector de modo + Start)
    ///     └── Panel_Client        (input de código + botón Unirse + feedback)
    ///
    /// Para conectar en el Editor:
    ///   1. Asigna todos los campos públicos desde el Inspector.
    ///   2. Coloca este script junto con LobbyManager, RelayManager y NetworkBootstrapper
    ///      en un GameObject de la escena de Lobby.
    /// </summary>
    public class LobbyUIController : MonoBehaviour
    {
        // ── Panel References ────────────────────────────────────────────────
        [Header("Panels")]
        public GameObject panelMain;
        public GameObject panelHost;
        public GameObject panelClient;

        // ── Panel_Main ───────────────────────────────────────────────────────
        [Header("Main Panel — Buttons")]
        public Button btnCreateRoom;
        public Button btnJoinRoom;

        // ── Panel_Host ───────────────────────────────────────────────────────
        [Header("Host Panel — Room Code Display")]
        [Tooltip("Large text that shows the 4-letter Room Code to share with friends.")]
        public Text txtRoomCode;
        [Tooltip("Status text showing how many players have connected.")]
        public Text txtPlayerCount;

        [Header("Host Panel — Game Mode Selector")]
        public Button btnModeAgencias;         // Agencias Rivales
        public Button btnModePolicia;          // Policía Corrupto
        private Color colorSelected   = new Color(0.2f, 0.8f, 0.4f);   // Green
        private Color colorUnselected = new Color(0.25f, 0.25f, 0.3f); // Dark grey

        [Header("Host Panel — Start")]
        public Button btnStartGame;
        [Tooltip("Shown when waiting for more players (< 2 connected).")]
        public Text txtWaitingForPlayers;
        public Button btnLeaveHost;

        // ── Panel_Client ─────────────────────────────────────────────────────
        [Header("Client Panel — Join Flow")]
        [Tooltip("4-character input field for the Room Code.")]
        public InputField inputRoomCode;
        public Button btnConfirmJoin;
        public Text txtJoinFeedback;
        public Button btnLeaveClient;

        // ── Internal State ───────────────────────────────────────────────────
        private bool isJoining = false;

        // ────────────────────────────────────────────────────────────────────
        #region Unity Lifecycle

        private void Start()
        {
            ShowPanel(panelMain);
            BindButtons();
            SubscribeToLobbyEvents();

            // Enforce 4-char limit on Room Code input
            if (inputRoomCode != null)
            {
                inputRoomCode.characterLimit = 4;
                inputRoomCode.onValueChanged.AddListener(OnRoomCodeInputChanged);
            }

            // Default game mode: Agencias Rivales
            UpdateModeButtonVisuals("AgenciasRivales");
        }

        private void OnDestroy()
        {
            UnsubscribeFromLobbyEvents();
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Button Binding

        private void BindButtons()
        {
            if (btnCreateRoom)  btnCreateRoom.onClick.AddListener(OnClickCreateRoom);
            if (btnJoinRoom)    btnJoinRoom.onClick.AddListener(OnClickJoinRoom);
            if (btnStartGame)   btnStartGame.onClick.AddListener(OnClickStartGame);
            if (btnConfirmJoin) btnConfirmJoin.onClick.AddListener(OnClickConfirmJoin);
            if (btnLeaveHost)   btnLeaveHost.onClick.AddListener(OnClickLeave);
            if (btnLeaveClient) btnLeaveClient.onClick.AddListener(OnClickLeave);
            if (btnModeAgencias) btnModeAgencias.onClick.AddListener(() => OnClickSelectMode("AgenciasRivales"));
            if (btnModePolicia)  btnModePolicia.onClick.AddListener(()  => OnClickSelectMode("Policiacorrupto"));
        }

        private void SubscribeToLobbyEvents()
        {
            if (LobbyManager.Instance == null) return;
            LobbyManager.Instance.OnRoomCreated       += OnRoomCreated;
            LobbyManager.Instance.OnRoomJoined        += OnRoomJoined;
            LobbyManager.Instance.OnPlayerCountChanged += OnPlayerCountChanged;
            LobbyManager.Instance.OnRoomError         += OnRoomError;
        }

        private void UnsubscribeFromLobbyEvents()
        {
            if (LobbyManager.Instance == null) return;
            LobbyManager.Instance.OnRoomCreated       -= OnRoomCreated;
            LobbyManager.Instance.OnRoomJoined        -= OnRoomJoined;
            LobbyManager.Instance.OnPlayerCountChanged -= OnPlayerCountChanged;
            LobbyManager.Instance.OnRoomError         -= OnRoomError;
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Button Handlers

        private async void OnClickCreateRoom()
        {
            ShowPanel(panelHost);
            if (txtRoomCode)    txtRoomCode.text    = "...";
            if (txtPlayerCount) txtPlayerCount.text = "Esperando jugadores...";
            if (btnStartGame)   btnStartGame.interactable = false;

            await LobbyManager.Instance.CreateRoomAsync(LobbyManager.Instance.SelectedGameMode);
        }

        private void OnClickJoinRoom()
        {
            ShowPanel(panelClient);
            if (txtJoinFeedback) txtJoinFeedback.text = "";
            if (inputRoomCode)   inputRoomCode.text    = "";
        }

        private async void OnClickConfirmJoin()
        {
            if (isJoining) return;
            string code = inputRoomCode != null ? inputRoomCode.text.Trim().ToUpper() : "";

            if (code.Length != 4)
            {
                if (txtJoinFeedback) txtJoinFeedback.text = "El código debe tener 4 letras.";
                return;
            }

            isJoining = true;
            if (btnConfirmJoin)  btnConfirmJoin.interactable = false;
            if (txtJoinFeedback) txtJoinFeedback.text = $"Buscando sala '{code}'...";

            await LobbyManager.Instance.JoinRoomAsync(code);

            isJoining = false;
            if (btnConfirmJoin) btnConfirmJoin.interactable = true;
        }

        private void OnClickSelectMode(string mode)
        {
            LobbyManager.Instance.SetGameMode(mode);
            UpdateModeButtonVisuals(mode);
        }

        private void OnClickStartGame()
        {
            LobbyManager.Instance.StartGame();
        }

        private async void OnClickLeave()
        {
            await LobbyManager.Instance.LeaveRoomAsync();
            ShowPanel(panelMain);
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region LobbyManager Event Callbacks

        private void OnRoomCreated(string roomCode)
        {
            if (txtRoomCode)
            {
                txtRoomCode.text = roomCode;
                // Animate the code appearing letter-by-letter for a nice reveal
                StartCoroutine(AnimateCodeReveal(roomCode));
            }
            if (txtPlayerCount) txtPlayerCount.text = "Jugadores: 1 / 6";
            RefreshStartButton();
        }

        private void OnRoomJoined()
        {
            // Client successfully joined — transition to waiting screen or directly to game
            if (txtJoinFeedback) txtJoinFeedback.text = $"¡Conectado a sala {LobbyManager.Instance.RoomCode}!";
            // The game scene load is triggered by LobbyManager/GameMode scripts via NetworkManager
        }

        private void OnPlayerCountChanged(int count)
        {
            if (txtPlayerCount) txtPlayerCount.text = $"Jugadores: {count} / 6";
            RefreshStartButton();
        }

        private void OnRoomError(string error)
        {
            Debug.LogWarning($"[LobbyUIController] Error: {error}");

            // Show error in whichever panel is visible
            if (panelHost.activeSelf   && txtPlayerCount) txtPlayerCount.text = $"⚠ {error}";
            if (panelClient.activeSelf && txtJoinFeedback) txtJoinFeedback.text = $"⚠ {error}";
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region UI Helpers

        private void ShowPanel(GameObject target)
        {
            if (panelMain   != null) panelMain.SetActive(panelMain     == target);
            if (panelHost   != null) panelHost.SetActive(panelHost     == target);
            if (panelClient != null) panelClient.SetActive(panelClient == target);
        }

        private void RefreshStartButton()
        {
            if (btnStartGame == null) return;
            bool enoughPlayers = LobbyManager.Instance.ConnectedPlayers >= 2;
            btnStartGame.interactable = enoughPlayers && LobbyManager.Instance.IsHost;
            if (txtWaitingForPlayers != null)
                txtWaitingForPlayers.gameObject.SetActive(!enoughPlayers);
        }

        private void UpdateModeButtonVisuals(string selectedMode)
        {
            if (btnModeAgencias != null)
                btnModeAgencias.image.color = selectedMode == "AgenciasRivales" ? colorSelected : colorUnselected;
            if (btnModePolicia != null)
                btnModePolicia.image.color = selectedMode == "PoliciaCorrupto" ? colorSelected : colorUnselected;
        }

        private void OnRoomCodeInputChanged(string value)
        {
            // Force uppercase as user types
            if (inputRoomCode != null && value != value.ToUpper())
            {
                int caretPos = inputRoomCode.caretPosition;
                inputRoomCode.text = value.ToUpper();
                inputRoomCode.caretPosition = caretPos;
            }
        }

        /// <summary>
        /// Animates the Room Code appearing one letter at a time (0.15s delay each).
        /// Gives a dramatic reveal effect — part of the "juicy" design philosophy.
        /// </summary>
        private IEnumerator AnimateCodeReveal(string code)
        {
            if (txtRoomCode == null) yield break;
            txtRoomCode.text = "";
            foreach (char c in code)
            {
                txtRoomCode.text += c;
                yield return new WaitForSeconds(0.15f);
            }
        }

        #endregion
    }
}
