using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Nigma.Editor
{
    public class MainMenuTransformer
    {
        [MenuItem("Nigma/1. Transformar Diseño del Menú Principal")]
        public static void TransformToMainMenu()
        {
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No se encontró un Canvas en la escena. Abre la escena del Lobby.");
                return;
            }

            Nigma.Networking.LobbyUIController lobbyUI = canvas.GetComponent<Nigma.Networking.LobbyUIController>();
            if (lobbyUI == null)
            {
                Debug.LogError("No se encontró el LobbyUIController.");
                return;
            }

            PremiumUIAssetGenerator.GenerateAssets();
            AssetDatabase.Refresh();

            Sprite roundedSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/UI/RoundedRect.png");
            Sprite woodSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/UI/WoodenRect.png");
            Font customFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/georgiab.ttf");
            if (customFont == null) customFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            Transform oldTitle = canvas.transform.Find("Title_Nigma");
            if (oldTitle != null) Object.DestroyImmediate(oldTitle.gameObject);

            GameObject titleObj = new GameObject("Title_Nigma");
            titleObj.transform.SetParent(canvas.transform, false);
            Text titleTxt = titleObj.AddComponent<Text>();
            titleTxt.text = "NIGMA";
            titleTxt.font = customFont;
            titleTxt.fontSize = 200;
            titleTxt.fontStyle = FontStyle.Bold;
            titleTxt.color = new Color(1f, 0.9f, 0.8f);
            titleTxt.alignment = TextAnchor.MiddleCenter;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -100);
            titleRect.sizeDelta = new Vector2(1000, 300);

            Shadow shadow = titleObj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.6f);
            shadow.effectDistance = new Vector2(0, -15);
            
            if (titleObj.GetComponent<Nigma.UI.FloatingTitle>() == null)
            {
                var ft = titleObj.AddComponent<Nigma.UI.FloatingTitle>();
                ft.amplitude = 15f;
            }

            Nigma.UI.MainMenuUIController mainUI = canvas.GetComponent<Nigma.UI.MainMenuUIController>();
            if (mainUI == null) mainUI = canvas.gameObject.AddComponent<Nigma.UI.MainMenuUIController>();

            Transform oldMainMenu = canvas.transform.Find("Panel_TrueMainMenu");
            if (oldMainMenu != null) Object.DestroyImmediate(oldMainMenu.gameObject);

            GameObject trueMainMenu = new GameObject("Panel_TrueMainMenu");
            trueMainMenu.transform.SetParent(canvas.transform, false);
            trueMainMenu.transform.SetSiblingIndex(2); 
            
            RectTransform tmRect = trueMainMenu.AddComponent<RectTransform>();
            tmRect.anchorMin = Vector2.zero;
            tmRect.anchorMax = Vector2.one;
            tmRect.sizeDelta = Vector2.zero;
            tmRect.anchoredPosition = Vector2.zero;

            // CASO DIARIO (Big button center)
            Button btnDaily = CreateButtonAnchored(trueMainMenu, "Btn_DailyCase", "CASO DIARIO", woodSprite, customFont,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -60), new Vector2(700, 180), 55);
            
            // BOTTOM BAR BUTTONS (CAMPAÑA, INFINITO, MULTIJUGADOR)
            Button btnCamp = CreateButtonAnchored(trueMainMenu, "Btn_Campaign", "CAMPAÑA", woodSprite, customFont,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-340, 120), new Vector2(320, 110), 30);
            
            Button btnEndless = CreateButtonAnchored(trueMainMenu, "Btn_Endless", "INFINITO", woodSprite, customFont,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 120), new Vector2(320, 110), 30);
            
            Button btnMulti = CreateButtonAnchored(trueMainMenu, "Btn_Multiplayer", "MULTIJUGADOR", woodSprite, customFont,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(340, 120), new Vector2(320, 110), 30);
            
            // AGENCY BUTTON (Top Left)
            Button btnAgency = CreateButtonAnchored(trueMainMenu, "Btn_Agency", "MI AGENCIA", woodSprite, customFont,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(240, -100), new Vector2(400, 100), 32);
            
            mainUI.panelMainMenu = trueMainMenu;
            mainUI.panelMultiplayerLobby = lobbyUI.panelMain;
            mainUI.btnDailyCase = btnDaily;
            mainUI.btnCampaign = btnCamp;
            mainUI.btnEndless = btnEndless;
            mainUI.btnMultiplayer = btnMulti;
            mainUI.btnAgency = btnAgency;

            // CONFIGURAR LOBBY MULTIJUGADOR
            StylePanel(lobbyUI.panelMain, roundedSprite);
            StylePanel(lobbyUI.panelHost, roundedSprite);
            StylePanel(lobbyUI.panelClient, roundedSprite);

            Transform oldBackBtn = lobbyUI.panelMain.transform.Find("Btn_BackToMenu");
            if (oldBackBtn != null) Object.DestroyImmediate(oldBackBtn.gameObject);
            
            Button btnBackMenu = CreateButtonAnchored(lobbyUI.panelMain.gameObject, "Btn_BackToMenu", "VOLVER AL MENÚ", woodSprite, customFont,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 100), new Vector2(650, 140), 40);
            btnBackMenu.onClick.AddListener(() => { mainUI.ShowMainMenu(); });

            StyleLobbyButton(lobbyUI.btnCreateRoom, "CREAR PARTIDA", woodSprite, customFont); 
            StyleLobbyButton(lobbyUI.btnJoinRoom, "UNIRSE A SALA", woodSprite, customFont);
            StyleLobbyButton(lobbyUI.btnStartGame, "EMPEZAR JUEGO", woodSprite, customFont); 
            StyleLobbyButton(lobbyUI.btnConfirmJoin, "CONECTAR", woodSprite, customFont);
            StyleLobbyButton(lobbyUI.btnLeaveHost, "ATRÁS", woodSprite, customFont); 
            StyleLobbyButton(lobbyUI.btnLeaveClient, "ATRÁS", woodSprite, customFont);

            if (lobbyUI.txtRoomCode != null)
            {
                lobbyUI.txtRoomCode.color = new Color(1f, 0.95f, 0.9f);
                lobbyUI.txtRoomCode.fontStyle = FontStyle.Bold;
                lobbyUI.txtRoomCode.fontSize = 140;
                if (lobbyUI.txtRoomCode.GetComponent<Shadow>() == null)
                {
                    var sh = lobbyUI.txtRoomCode.gameObject.AddComponent<Shadow>();
                    sh.effectDistance = new Vector2(0, -5);
                    sh.effectColor = new Color(0,0,0,0.5f);
                }
            }

            if (lobbyUI.inputRoomCode != null)
            {
                Image inputImg = lobbyUI.inputRoomCode.GetComponent<Image>();
                inputImg.sprite = roundedSprite;
                inputImg.type = Image.Type.Sliced;
                inputImg.color = new Color(1f, 1f, 1f, 0.95f); 
                
                Text placeholder = lobbyUI.inputRoomCode.placeholder as Text;
                if (placeholder != null) placeholder.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                Text textComp = lobbyUI.inputRoomCode.textComponent;
                if (textComp != null)
                {
                    textComp.color = new Color(0.2f, 0.15f, 0.1f);
                    textComp.fontSize = 80;
                    textComp.alignment = TextAnchor.MiddleCenter;
                }
                
                RectTransform inputRect = lobbyUI.inputRoomCode.GetComponent<RectTransform>();
                inputRect.sizeDelta = new Vector2(600, 150);
            }

            Generate3DBackground();

            // ASEGURAR QUE LOS PANELES NO SE SOLAPEN EN EL EDITOR
            if (lobbyUI.panelMain != null) lobbyUI.panelMain.SetActive(false);
            if (lobbyUI.panelHost != null) lobbyUI.panelHost.SetActive(false);
            if (lobbyUI.panelClient != null) lobbyUI.panelClient.SetActive(false);
            trueMainMenu.SetActive(true);
            
            mainUI.ShowMainMenu();

            EditorUtility.SetDirty(lobbyUI);
            EditorUtility.SetDirty(mainUI);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );

            Debug.Log("[MainMenuTransformer] Menú actualizado con UI profesional.");
        }

        private static void Generate3DBackground()
        {
            GameObject oldDiorama = GameObject.Find("Lobby_3D_Diorama");
            if (oldDiorama != null) Object.DestroyImmediate(oldDiorama);
            
            GameObject oldPivot = GameObject.Find("Lobby_3D_Pivot");
            if (oldPivot != null) Object.DestroyImmediate(oldPivot);

            var generator = ScriptableObject.CreateInstance<PlayableSceneGenerator>();
            GameObject board = generator.GenerateVisualBoard(true);
            Object.DestroyImmediate(generator);

            board.name = "Lobby_3D_Diorama";
            
            board.transform.position = new Vector3(-3.75f, 0, -3.75f);
            
            GameObject pivot = new GameObject("Lobby_3D_Pivot");
            pivot.transform.position = Vector3.zero;
            board.transform.SetParent(pivot.transform);

            if (pivot.GetComponent<Nigma.UI.BackgroundRotator>() == null)
            {
                var rot = pivot.AddComponent<Nigma.UI.BackgroundRotator>();
                rot.rotationSpeed = -4f;
            }

            if (pivot.GetComponent<Nigma.UI.LobbyPostProcessing>() == null)
            {
                pivot.AddComponent<Nigma.UI.LobbyPostProcessing>();
            }

            GameObject lightObj = new GameObject("Diorama_Light");
            lightObj.transform.parent = pivot.transform;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.93f, 0.80f);
            light.intensity = 1.3f;
            
            // ACERCAR LA CÁMARA EL DOBLE Y CENTRARLA
            if (Camera.main != null)
            {
                Camera.main.backgroundColor = new Color(0.08f, 0.05f, 0.04f); 
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
                Camera.main.transform.position = new Vector3(0, 13, -9);
                Camera.main.transform.rotation = Quaternion.Euler(55, 0, 0);
            }
        }

        private static Button CreateButtonAnchored(GameObject parent, string name, string label, Sprite woodSprite, Font customFont, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, int fontSize = 46)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent.transform, false);
            Button btn = btnObj.AddComponent<Button>();
            
            Image img = btnObj.AddComponent<Image>();
            img.sprite = woodSprite;
            img.type = Image.Type.Sliced;
            img.color = Color.white; 
            
            Shadow shadow = btnObj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.35f);
            shadow.effectDistance = new Vector2(0, -5);

            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = size;

            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            
            Text txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.font = customFont;
            txt.fontSize = fontSize;
            txt.fontStyle = FontStyle.Bold;
            // Profesor Layton goldish-parchment text color
            txt.color = new Color(0.96f, 0.88f, 0.72f);
            txt.alignment = TextAnchor.MiddleCenter;
            
            RectTransform txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.sizeDelta = Vector2.zero;
            
            Shadow txtShadow = txtObj.AddComponent<Shadow>();
            txtShadow.effectColor = new Color(0, 0, 0, 0.7f);
            txtShadow.effectDistance = new Vector2(1, -2);
            
            btnObj.AddComponent<Nigma.Core.ButtonJuice>();

            return btn;
        }

        private static void StylePanel(GameObject panel, Sprite roundedSprite)
        {
            if (panel == null) return;
            
            Image img = panel.GetComponent<Image>();
            if (img != null) Object.DestroyImmediate(img);
            Outline outline = panel.GetComponent<Outline>();
            if (outline != null) Object.DestroyImmediate(outline);
            
            RectTransform rect = panel.GetComponent<RectTransform>();
            if (rect == null) rect = panel.AddComponent<RectTransform>();
            
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0, -50); 
            rect.sizeDelta = new Vector2(850, 1400); 

            VerticalLayoutGroup vlg = panel.GetComponent<VerticalLayoutGroup>();
            if (vlg == null) vlg = panel.AddComponent<VerticalLayoutGroup>();
            
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.spacing = 40; 
            vlg.padding = new RectOffset(60, 60, 60, 60); 
            vlg.childControlHeight = false;
            vlg.childControlWidth = false;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = false;
        }

        private static void StyleLobbyButton(Button btn, string label, Sprite woodSprite, Font customFont)
        {
            if (btn == null) return;

            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(650, 140);

            Image img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = woodSprite;
                img.type = Image.Type.Sliced;
                img.color = Color.white;
                
                Shadow shadow = btn.GetComponent<Shadow>();
                if (shadow == null) shadow = btn.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0, 0, 0, 0.35f);
                shadow.effectDistance = new Vector2(0, -5);

                Outline outline = btn.GetComponent<Outline>();
                if (outline != null) Object.DestroyImmediate(outline);
            }

            Text txt = btn.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = label;
                txt.font = customFont;
                txt.fontSize = 46;
                txt.fontStyle = FontStyle.Normal;
                txt.color = new Color(1f, 0.95f, 0.9f);
                
                Shadow txtShadow = txt.GetComponent<Shadow>();
                if (txtShadow == null) txtShadow = txt.gameObject.AddComponent<Shadow>();
                txtShadow.effectColor = new Color(0, 0, 0, 0.6f);
                txtShadow.effectDistance = new Vector2(1, -1);
            }
        }
    }
}
