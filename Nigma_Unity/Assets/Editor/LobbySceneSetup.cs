using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Nigma.Networking;

namespace Nigma.Editor
{
    public class LobbySceneSetup
    {
        [MenuItem("Nigma/Configuración Automática/1. Crear Escena de Lobby", false, 1)]
        public static void CreateLobbyScene()
        {
            // 1. Create a new empty scene
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // Create a directional light and camera just in case
            GameObject cameraObj = new GameObject("Main Camera");
            Camera cam = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
            cameraObj.transform.position = new Vector3(0, 1, -10);

            // 2. Create NetworkManager (Needs Unity.Netcode package)
            GameObject netManagerObj = new GameObject("NetworkManager");
            
            // Try to add Netcode NetworkManager via reflection if available
            System.Type netManagerType = System.Type.GetType("Unity.Netcode.NetworkManager, Unity.Netcode.Runtime");
            if (netManagerType != null)
            {
                netManagerObj.AddComponent(netManagerType);
            }
            else
            {
                Debug.LogWarning("Netcode package no detectado. Tendrás que añadir el NetworkManager manualmente cuando lo instales.");
            }
            
            netManagerObj.AddComponent<NetworkBootstrapper>();

            // 3. Create LobbySystem
            GameObject lobbySystemObj = new GameObject("LobbySystem");
            lobbySystemObj.AddComponent<RelayManager>();
            lobbySystemObj.AddComponent<LobbyManager>();

            // 4. Create UI Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Create EventSystem
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();

            // Attach LobbyUIController
            LobbyUIController uiController = canvasObj.AddComponent<LobbyUIController>();

            // 5. Create UI Hierarchy
            GameObject panelMain = CreatePanel(canvasObj, "Panel_Main");
            GameObject panelHost = CreatePanel(canvasObj, "Panel_Host");
            GameObject panelClient = CreatePanel(canvasObj, "Panel_Client");
            
            panelHost.SetActive(false);
            panelClient.SetActive(false);

            // --- Panel_Main UI ---
            Button btnCreateRoom = CreateButton(panelMain, "Btn_CreateRoom", "Crear Sala", new Vector2(0, 50));
            Button btnJoinRoom = CreateButton(panelMain, "Btn_JoinRoom", "Unirse a Sala", new Vector2(0, -50));

            // --- Panel_Host UI ---
            Text txtRoomCode = CreateText(panelHost, "Txt_RoomCode", "XXXX", 60, new Vector2(0, 150));
            txtRoomCode.alignment = TextAnchor.MiddleCenter;
            Text txtPlayerCount = CreateText(panelHost, "Txt_PlayerCount", "Jugadores: 1 / 6", 24, new Vector2(0, 70));
            txtPlayerCount.alignment = TextAnchor.MiddleCenter;
            
            Button btnModeAgencias = CreateButton(panelHost, "Btn_ModeAgencias", "Agencias Rivales", new Vector2(-120, -20));
            Button btnModePolicia = CreateButton(panelHost, "Btn_ModePolicia", "Policía Corrupto", new Vector2(120, -20));
            
            Text txtWaiting = CreateText(panelHost, "Txt_Waiting", "Esperando más jugadores...", 20, new Vector2(0, -90));
            txtWaiting.alignment = TextAnchor.MiddleCenter;
            txtWaiting.color = Color.yellow;
            Button btnStartGame = CreateButton(panelHost, "Btn_StartGame", "Empezar Partida", new Vector2(0, -150));
            Button btnLeaveHost = CreateButton(panelHost, "Btn_LeaveHost", "Salir", new Vector2(0, -220));

            // --- Panel_Client UI ---
            Text txtJoinPrompt = CreateText(panelClient, "Txt_Prompt", "Introduce el código de sala:", 24, new Vector2(0, 100));
            txtJoinPrompt.alignment = TextAnchor.MiddleCenter;
            
            InputField inputRoomCode = CreateInputField(panelClient, "Input_RoomCode", new Vector2(0, 30));
            Button btnConfirmJoin = CreateButton(panelClient, "Btn_ConfirmJoin", "Conectar", new Vector2(0, -50));
            Text txtJoinFeedback = CreateText(panelClient, "Txt_Feedback", "", 20, new Vector2(0, -110));
            txtJoinFeedback.alignment = TextAnchor.MiddleCenter;
            Button btnLeaveClient = CreateButton(panelClient, "Btn_LeaveClient", "Volver", new Vector2(0, -180));

            // 6. Connect References
            uiController.panelMain = panelMain;
            uiController.panelHost = panelHost;
            uiController.panelClient = panelClient;
            
            uiController.btnCreateRoom = btnCreateRoom;
            uiController.btnJoinRoom = btnJoinRoom;
            
            uiController.txtRoomCode = txtRoomCode;
            uiController.txtPlayerCount = txtPlayerCount;
            uiController.btnModeAgencias = btnModeAgencias;
            uiController.btnModePolicia = btnModePolicia;
            uiController.txtWaitingForPlayers = txtWaiting;
            uiController.btnStartGame = btnStartGame;
            uiController.btnLeaveHost = btnLeaveHost;
            
            uiController.inputRoomCode = inputRoomCode;
            uiController.btnConfirmJoin = btnConfirmJoin;
            uiController.txtJoinFeedback = txtJoinFeedback;
            uiController.btnLeaveClient = btnLeaveClient;

            // 7. Save Scene
            string scenePath = "Assets/Scenes/LobbyScene.unity";
            
            // Ensure folder exists
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            EditorSceneManager.SaveScene(newScene, scenePath);
            Debug.Log($"[Nigma] ¡Escena de Lobby creada con éxito en {scenePath}!");
            
            // Add to Build Settings if not there
            AddSceneToBuildSettings(scenePath);
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var originalScenes = EditorBuildSettings.scenes;
            bool found = false;
            foreach (var scene in originalScenes)
            {
                if (scene.path == scenePath)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                var newScenes = new EditorBuildSettingsScene[originalScenes.Length + 1];
                // Put Lobby at index 0
                newScenes[0] = new EditorBuildSettingsScene(scenePath, true);
                System.Array.Copy(originalScenes, 0, newScenes, 1, originalScenes.Length);
                EditorBuildSettings.scenes = newScenes;
                Debug.Log("[Nigma] Escena de Lobby añadida como la primera escena en Build Settings.");
            }
        }

        // --- Helper Methods to generate UI ---

        private static GameObject CreatePanel(GameObject parent, string name)
        {
            GameObject panelObj = new GameObject(name);
            panelObj.transform.SetParent(parent.transform, false);
            RectTransform rect = panelObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            Image img = panelObj.AddComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Dark background
            return panelObj;
        }

        private static Button CreateButton(GameObject parent, string name, string text, Vector2 pos)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent.transform, false);
            RectTransform rect = btnObj.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(200, 50);
            
            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            Button btn = btnObj.AddComponent<Button>();

            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            RectTransform txtRect = txtObj.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;
            Text txt = txtObj.AddComponent<Text>();
            txt.text = text;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.fontSize = 20;

            return btn;
        }

        private static Text CreateText(GameObject parent, string name, string text, int fontSize, Vector2 pos)
        {
            GameObject txtObj = new GameObject(name);
            txtObj.transform.SetParent(parent.transform, false);
            RectTransform rect = txtObj.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(400, 50);
            
            Text txt = txtObj.AddComponent<Text>();
            txt.text = text;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.color = Color.white;
            txt.fontSize = fontSize;
            return txt;
        }

        private static InputField CreateInputField(GameObject parent, string name, Vector2 pos)
        {
            GameObject inputObj = new GameObject(name);
            inputObj.transform.SetParent(parent.transform, false);
            RectTransform rect = inputObj.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(200, 50);
            
            Image img = inputObj.AddComponent<Image>();
            img.color = Color.white;
            InputField inputField = inputObj.AddComponent<InputField>();

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(inputObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 0);
            textRect.offsetMax = new Vector2(-10, 0);
            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.black;
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleCenter;

            inputField.textComponent = text;
            return inputField;
        }
    }
}
