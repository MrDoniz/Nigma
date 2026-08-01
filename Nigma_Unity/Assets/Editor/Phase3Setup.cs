#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Nigma.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Phase3Setup : EditorWindow
{
    [MenuItem("Nigma/2. Arreglar Cámara e Input (Pulsar aquí)")]
    public static void FixCameraAndInput()
    {
        // Fix Camera for Isometric View
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(-5f, 10f, -5f);
            cam.transform.rotation = Quaternion.Euler(30f, 45f, 0f);
        }

        // Fix EventSystem for New Input System
        EventSystem es = FindObjectOfType<EventSystem>();
        if (es != null)
        {
            var oldModule = es.GetComponent<StandaloneInputModule>();
            if (oldModule != null)
            {
                DestroyImmediate(oldModule);
                // We add the generic component type by name to avoid namespace errors if InputSystem package isn't loaded correctly in scripts
                es.gameObject.AddComponent(System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem"));
            }
        }
        
        Debug.Log("¡Cámara isométrica e Input arreglados!");
    }

    [MenuItem("Nigma/1. Configurar Fase 3 Automáticamente")]
    public static void SetupPhase3()
    {
        // 1. Create LevelData Assets
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
        {
            AssetDatabase.CreateFolder("Assets", "Data");
        }

        List<LevelData> createdLevels = new List<LevelData>();

        for (int i = 1; i <= 3; i++)
        {
            string path = $"Assets/Data/Level{i}.asset";
            LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (level == null)
            {
                level = ScriptableObject.CreateInstance<LevelData>();
                level.levelID = i;
                level.puzzleDescription = i == 1 ? "Acertijo 1 (Fácil): ¿Dónde está la tarta?" : (i == 2 ? "Acertijo 2 (Medio): El sospechoso..." : "Acertijo 3 (Difícil): Misterio final");
                level.correctAnswer = new Vector2Int(i, i); // Placeholder answers
                level.availableMirrors = i;
                level.availableSofas = 1;
                AssetDatabase.CreateAsset(level, path);
            }
            createdLevels.Add(level);
        }
        AssetDatabase.SaveAssets();

        // 2. Setup Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            DestroyImmediate(canvas.gameObject);
        }
        
        GameObject canvasObj = new GameObject("Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        // 4. Create UI Elements
        GameObject txtAtestadoObj = CreateText("Txt_Atestado", canvas.transform, new Vector2(0, 200), new Vector2(600, 200));
        GameObject txtEspejosObj = CreateText("Txt_Espejos", canvas.transform, new Vector2(-300, -200), new Vector2(200, 50));
        GameObject txtSofasObj = CreateText("Txt_Sofas", canvas.transform, new Vector2(-300, -250), new Vector2(200, 50));
        GameObject txtFeedbackObj = CreateText("Txt_Feedback", canvas.transform, new Vector2(0, 50), new Vector2(500, 100));
        
        GameObject btnResolverObj = new GameObject("Btn_Resolver");
        btnResolverObj.transform.SetParent(canvas.transform, false);
        btnResolverObj.AddComponent<RectTransform>().anchoredPosition = new Vector2(200, -150);
        btnResolverObj.AddComponent<Image>().color = Color.white;
        Button btnResolver = btnResolverObj.AddComponent<Button>();
        CreateText("Text", btnResolverObj.transform, Vector2.zero, new Vector2(100, 30)).GetComponent<Text>().text = "Resolver";
        
        GameObject panelVictoryObj = new GameObject("Panel_Victory");
        panelVictoryObj.transform.SetParent(canvas.transform, false);
        panelVictoryObj.AddComponent<RectTransform>().sizeDelta = new Vector2(300, 200);
        panelVictoryObj.AddComponent<Image>().color = new Color(0, 0.5f, 0, 0.5f);
        CreateText("Txt_Vic", panelVictoryObj.transform, new Vector2(0, 50), new Vector2(300, 50)).GetComponent<Text>().text = "¡Victoria!";

        // 5. Setup UIManager
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            GameObject uiManagerObj = new GameObject("UIManager");
            uiManager = uiManagerObj.AddComponent<UIManager>();
        }
        uiManager.puzzleTextUI = txtAtestadoObj.GetComponent<Text>();
        uiManager.feedbackTextUI = txtFeedbackObj.GetComponent<Text>();
        uiManager.mirrorsCountText = txtEspejosObj.GetComponent<Text>();
        uiManager.sofasCountText = txtSofasObj.GetComponent<Text>();
        uiManager.victoryPanel = panelVictoryObj;

        // 5. Setup GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gm = gmObj.AddComponent<GameManager>();
        }
        gm.uiManager = uiManager;
        gm.levels = createdLevels;

        // Setup the button click event
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnResolver.onClick, gm.OnSolveButtonClicked);

        // 6. Setup GridManager
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            GameObject gridObj = new GameObject("GridManager");
            gridManager = gridObj.AddComponent<GridManager>();
        }
        
        if (gridManager.structuralWallPrefab == null)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gridManager.structuralWallPrefab = cube;
            cube.SetActive(false); // Hide the primitive in scene
            cube.name = "Placeholder_Wall_Prefab";
        }

        panelVictoryObj.SetActive(false);

        Debug.Log("¡Nigma: Fase 3 configurada con éxito! Revisa tu escena.");
    }

    private static GameObject CreateText(string name, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Text text = obj.AddComponent<Text>();
        text.text = name;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.fontSize = 20;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        
        // Add a background so white text is visible if scene is light
        Outline outline = obj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1, -1);
        
        return obj;
    }

    // ─────────────────────────────────────────────────────────────────────────
    [MenuItem("Nigma/4. Actualizar Escena Completa (Puzzles + Jokers + Safe)")]
    public static void FullSceneUpdate()
    {
        // ── 1. Create real LevelData assets ──────────────────────────────────
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
            AssetDatabase.CreateFolder("Assets", "Data");

        LevelData level1 = GetOrCreateLevel("Assets/Data/Level1.asset");
        level1.levelID = 1;
        level1.puzzleDescription =
            "🔎 CASO 1: «La Tarta Desaparecida»\n\n" +
            "La señora Miau dejó su tarta en la cocina y al volver la encontró a medias.\n" +
            "El Gato jura que estaba en el sofá y vio perfectamente la cocina todo el tiempo.\n" +
            "Pero el Perro insiste que el sofá bloqueaba la vista hacia el lado derecho.\n\n" +
            "¿En qué casilla estaba la tarta cuando desapareció?\n" +
            "(Coloca el Sofá en la casilla central y deduce la línea de visión del Gato)";
        level1.correctAnswer    = new Vector2Int(3, 1);
        level1.availableMirrors = 0;
        level1.availableSofas   = 1;
        level1.structuralWalls  = new List<Vector2Int>();
        level1.safeCode = ""; level1.safeHint = "";
        EditorUtility.SetDirty(level1);

        LevelData level2 = GetOrCreateLevel("Assets/Data/Level2.asset");
        level2.levelID = 2;
        level2.puzzleDescription =
            "🔎 CASO 2: «El Robo en la Galería»\n\n" +
            "Esta noche han robado el cuadro más valioso del museo.\n" +
            "El vigilante Búho asegura que vigilaba desde el pasillo con línea de visión directa.\n" +
            "La directora Lechuza señala que hay un espejo en la sala que desviaba su vista.\n" +
            "El ladrón aprovechó ese ángulo muerto.\n\n" +
            "¿En qué casilla estaba el cuadro cuando lo cogieron?\n" +
            "(El muro gris bloquea el acceso. Usa el Espejo para reconstruir el ángulo del Búho)";
        level2.correctAnswer    = new Vector2Int(2, 3);
        level2.availableMirrors = 1;
        level2.availableSofas   = 1;
        level2.structuralWalls  = new List<Vector2Int> { new Vector2Int(1, 1) };
        level2.safeCode = "23";
        level2.safeHint = "🔒 Código = Coordenada X de la solución seguida de Y (dos dígitos)";
        EditorUtility.SetDirty(level2);

        LevelData level3 = GetOrCreateLevel("Assets/Data/Level3.asset");
        level3.levelID = 3;
        level3.puzzleDescription =
            "🔎 CASO 3: «El Testigo Ciego»\n\n" +
            "Robo con violencia en el piso 3. Dos testigos que se contradicen.\n" +
            "La Zorra jura haber visto al culpable cruzar el pasillo central a plena luz.\n" +
            "El Conejo dice que los dos muros laterales lo hacen imposible: ángulo muerto total.\n" +
            "Los espejos podían reflejar la visión... o precisamente ocultarla.\n\n" +
            "¿En qué casilla estaba el sospechoso cuando fue visto?\n" +
            "(Coloca los 2 Espejos para reconstruir la cadena de reflexión posible de la Zorra)";
        level3.correctAnswer    = new Vector2Int(1, 3);
        level3.availableMirrors = 2;
        level3.availableSofas   = 1;
        level3.structuralWalls  = new List<Vector2Int> { new Vector2Int(0, 2), new Vector2Int(3, 2) };
        level3.safeCode = ""; level3.safeHint = "";
        EditorUtility.SetDirty(level3);

        AssetDatabase.SaveAssets();

        // ── 2. Assign levels to GameManager ──────────────────────────────────
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.levels = new List<LevelData> { level1, level2, level3 };
            gm.currentLevelIndex = 0;
            EditorUtility.SetDirty(gm);
            Debug.Log("[Nigma] LevelData asignados al GameManager.");
        }
        else
        {
            Debug.LogWarning("[Nigma] GameManager no encontrado en la escena. Asigna los niveles manualmente.");
        }

        // ── 3. Add JokerManager if missing ────────────────────────────────────
        JokerManager jm = FindObjectOfType<JokerManager>();
        if (jm == null)
        {
            GameObject jokerObj = new GameObject("JokerManager");
            jokerObj.AddComponent<JokerManager>();
            Debug.Log("[Nigma] JokerManager creado.");
        }

        // ── 4. Add SafeManager if missing ─────────────────────────────────────
        SafeManager sm = FindObjectOfType<SafeManager>();
        if (sm == null)
        {
            GameObject safeObj = new GameObject("SafeManager");
            safeObj.AddComponent<SafeManager>();
            Debug.Log("[Nigma] SafeManager creado.");
        }

        Debug.Log("✅ [Nigma] Escena actualizada. Dale al Play para probar los puzzles reales.");
    }

    private static LevelData GetOrCreateLevel(string path)
    {
        LevelData existing = AssetDatabase.LoadAssetAtPath<LevelData>(path);
        if (existing != null) return existing;
        LevelData newLevel = ScriptableObject.CreateInstance<LevelData>();
        AssetDatabase.CreateAsset(newLevel, path);
        return newLevel;
    }
}
#endif
