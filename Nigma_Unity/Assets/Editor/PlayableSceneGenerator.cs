using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

namespace Nigma.Editor
{
    public class PlayableSceneGenerator : EditorWindow
    {
        [MenuItem("Nigma/2. Generar Escena Jugable", false, 2)]
        public static void ShowWindow()
        {
            var window = GetWindow<PlayableSceneGenerator>("Generador Escena Jugable");
            window.minSize = new Vector2(300, 200);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Generador de Prototipo Jugable", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Genera un nivel completo estilo Murdoku con habitaciones, muebles y sospechosos.", MessageType.Info);
            if (GUILayout.Button("Generar Escena Completa", GUILayout.Height(40)))
            {
                GeneratePlayableScene();
                EditorUtility.DisplayDialog("Éxito", "Escena generada. Dale a Play.", "OK");
            }
        }

        // ─── Cached Materials ──────────────────────────────────────────
        private Material matDarkWood, matLightWood, matWall, matWallTranslucent;
        private Material matTrim, matBase;
        private Material matSofa, matBed, matTable, matPlant, matPlantPot;
        private Material matChair, matRug, matLamp, matLampShade;
        private Material matSuspect1, matSuspect2, matSuspect3, matSuspect4;
        private Material matSkin, matSkinDark, matHat;
        private Material matGlass, matDoorFrame, matDoor, matWindowFrame;
        private Material matHairBlonde, matHairBrown, matHairRed, matHairDark;
        private Material matShoes, matPants, matSkirt;

        private static Material MakeMat(Color color, float smooth = 0.5f, float metal = 0f)
        {
            Shader urp = Shader.Find("Universal Render Pipeline/Lit");
            if (urp == null) urp = Shader.Find("Standard");
            Material m = new Material(urp);
            m.SetColor("_BaseColor", color);
            m.color = color;
            m.SetFloat("_Smoothness", smooth);
            m.SetFloat("_Metallic", metal);
            return m;
        }

        private static Material MakeTransparentMat(Color color, float smooth = 0.3f)
        {
            Shader urp = Shader.Find("Universal Render Pipeline/Lit");
            if (urp == null) urp = Shader.Find("Standard");
            Material m = new Material(urp);
            // Enable transparency
            m.SetFloat("_Surface", 1); // Transparent
            m.SetFloat("_Blend", 0);   // Alpha
            m.SetOverrideTag("RenderType", "Transparent");
            m.renderQueue = 3000;
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            m.SetColor("_BaseColor", color);
            m.color = color;
            m.SetFloat("_Smoothness", smooth);
            return m;
        }

        private void InitMaterials()
        {
            // Floor
            matDarkWood  = MakeMat(new Color(0.32f, 0.20f, 0.11f), 0.7f);
            matLightWood = MakeMat(new Color(0.52f, 0.36f, 0.20f), 0.6f);
            matBase      = MakeMat(new Color(0.15f, 0.10f, 0.06f), 0.4f);

            // Walls
            matWall            = MakeMat(new Color(0.28f, 0.22f, 0.16f), 0.3f);
            matWallTranslucent = MakeTransparentMat(new Color(0.28f, 0.22f, 0.16f, 0.45f));
            matTrim            = MakeMat(new Color(0.72f, 0.56f, 0.28f), 0.8f, 0.15f);
            matGlass           = MakeTransparentMat(new Color(0.6f, 0.75f, 0.9f, 0.25f), 0.9f);
            matWindowFrame     = MakeMat(new Color(0.5f, 0.38f, 0.22f), 0.6f);
            matDoorFrame       = MakeMat(new Color(0.42f, 0.30f, 0.18f), 0.5f);
            matDoor            = MakeMat(new Color(0.35f, 0.24f, 0.14f), 0.4f);

            // Furniture
            matSofa      = MakeMat(new Color(0.55f, 0.22f, 0.10f), 0.55f);
            matBed       = MakeMat(new Color(0.85f, 0.82f, 0.75f), 0.3f);
            matTable     = MakeMat(new Color(0.40f, 0.28f, 0.15f), 0.6f);
            matChair     = MakeMat(new Color(0.45f, 0.30f, 0.18f), 0.5f);
            matRug       = MakeMat(new Color(0.6f, 0.15f, 0.12f), 0.2f);
            matPlant     = MakeMat(new Color(0.2f, 0.55f, 0.15f), 0.3f);
            matPlantPot  = MakeMat(new Color(0.6f, 0.35f, 0.18f), 0.4f);
            matLamp      = MakeMat(new Color(0.3f, 0.25f, 0.2f), 0.5f, 0.2f);
            matLampShade = MakeMat(new Color(0.95f, 0.88f, 0.65f), 0.2f);

            // Suspects clothing
            matSuspect1 = MakeMat(new Color(0.2f, 0.45f, 0.75f), 0.4f);
            matSuspect2 = MakeMat(new Color(0.7f, 0.2f, 0.2f), 0.4f);
            matSuspect3 = MakeMat(new Color(0.2f, 0.6f, 0.3f), 0.4f);
            matSuspect4 = MakeMat(new Color(0.6f, 0.4f, 0.7f), 0.4f);
            matSkin     = MakeMat(new Color(0.88f, 0.75f, 0.60f), 0.35f);
            matSkinDark = MakeMat(new Color(0.62f, 0.45f, 0.32f), 0.35f);
            matHat      = MakeMat(new Color(0.18f, 0.14f, 0.10f), 0.5f);
            matHairBlonde = MakeMat(new Color(0.9f, 0.8f, 0.5f), 0.3f);
            matHairBrown  = MakeMat(new Color(0.3f, 0.2f, 0.1f), 0.3f);
            matHairRed    = MakeMat(new Color(0.7f, 0.25f, 0.1f), 0.3f);
            matHairDark   = MakeMat(new Color(0.12f, 0.08f, 0.05f), 0.3f);
            matShoes      = MakeMat(new Color(0.15f, 0.1f, 0.08f), 0.5f);
            matPants      = MakeMat(new Color(0.2f, 0.18f, 0.25f), 0.3f);
            matSkirt      = MakeMat(new Color(0.5f, 0.15f, 0.15f), 0.3f);
        }

        // ═══════════════════════════════════════════════════════════════
        // MAIN GENERATION
        // ═══════════════════════════════════════════════════════════════
        private void GeneratePlayableScene()
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            newScene.name = "GameplayPrototype";

            InitMaterials();

            // ─── Camera (high angle, orthographic) ─────────────────────
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            Camera cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 8f;
            // Higher angle (50°) so we look more top-down into rooms
            cam.transform.position = new Vector3(-7f, 12f, -7f);
            cam.transform.rotation = Quaternion.Euler(45f, 45f, 0f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.10f, 0.09f, 0.12f);
            camObj.AddComponent<PhysicsRaycaster>();

            // ─── Lighting ──────────────────────────────────────────────
            GameObject mainLightObj = new GameObject("Main Light");
            Light mainLight = mainLightObj.AddComponent<Light>();
            mainLight.type = LightType.Directional;
            mainLightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            mainLight.color = new Color(1f, 0.93f, 0.80f);
            mainLight.intensity = 1.2f;
            mainLight.shadows = LightShadows.Soft;

            GameObject fillObj = new GameObject("Fill Light");
            Light fill = fillObj.AddComponent<Light>();
            fill.type = LightType.Directional;
            fillObj.transform.rotation = Quaternion.Euler(25f, 150f, 0f);
            fill.color = new Color(0.55f, 0.6f, 0.75f);
            fill.intensity = 0.35f;
            fill.shadows = LightShadows.None;

            // ─── Grid & Board generation moved to helper ───
            GameObject gridObj = GenerateVisualBoard(true);
            var gm = gridObj.GetComponent<Nigma.Core.GridManager>();



            // ─── EVENT SYSTEM ──────────────────────────────────────────
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

            // ─── UI ────────────────────────────────────────────────────
            BuildUI(cam, gm);

            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        public GameObject GenerateVisualBoard(bool includeSuspects = false)
        {
            InitMaterials();
            
            GameObject gridObj = new GameObject("GridManager");
            var gm = gridObj.AddComponent<Nigma.Core.GridManager>();
            gm.width = 6;
            gm.height = 6;
            gm.cellSize = 1.5f;
            float cs = gm.cellSize;

            // ─── Board & Floor ─────────────────────────────────────────
            GameObject board = new GameObject("VisualBoard");
            board.transform.SetParent(gridObj.transform);

            // Checkerboard floor tiles
            for (int x = 0; x < gm.width; x++)
            {
                for (int y = 0; y < gm.height; y++)
                {
                    Vector3 pos = gm.GetWorldPosition(x, y);
                    var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    tile.name = $"Tile_{x}_{y}";
                    tile.transform.SetParent(board.transform);
                    tile.transform.position = pos + Vector3.down * 0.16f;
                    tile.transform.localScale = new Vector3(cs - 0.04f, 0.3f, cs - 0.04f);
                    tile.GetComponent<Renderer>().sharedMaterial = (x + y) % 2 == 0 ? matDarkWood : matLightWood;
                }
            }

            // Base slab
            float bw = gm.width * cs;
            float bh = gm.height * cs;
            Vector3 center = new Vector3((gm.width - 1) * cs * 0.5f, -0.52f, (gm.height - 1) * cs * 0.5f);
            var slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slab.name = "BaseSlab";
            slab.transform.SetParent(board.transform);
            slab.transform.position = center;
            slab.transform.localScale = new Vector3(bw + 0.6f, 0.4f, bh + 0.6f);
            slab.GetComponent<Renderer>().sharedMaterial = matBase;

            // ─── WALLS ─────────────────────────────────────────────────
            GameObject wallsParent = new GameObject("Walls");
            wallsParent.transform.SetParent(board.transform);

            float wallH = 0.9f;
            float wallT = 0.12f;
            float halfCS = cs * 0.5f;
            float xMin = -halfCS;
            float xMax = (gm.width - 1) * cs + halfCS;
            float zMin = -halfCS;
            float zMax = (gm.height - 1) * cs + halfCS;
            float midX = (xMin + xMax) * 0.5f;
            float midZ = (zMin + zMax) * 0.5f;
            float fullW = xMax - xMin;
            float fullH = zMax - zMin;

            // ── Exterior walls with windows ──
            BuildExteriorWallWithWindows(wallsParent, "Wall_Back", matWall, false,
                new Vector3(midX, 0, zMax + wallT * 0.5f), fullW + wallT * 2, wallH, wallT, 2);
            BuildExteriorWallWithWindows(wallsParent, "Wall_Left", matWall, true,
                new Vector3(xMin - wallT * 0.5f, 0, midZ), fullH + wallT * 2, wallH, wallT, 2);
            BuildExteriorWallWithWindows(wallsParent, "Wall_Front", matWallTranslucent, false,
                new Vector3(midX, 0, zMin - wallT * 0.5f), fullW + wallT * 2, wallH, wallT, 2);
            BuildExteriorWallWithWindows(wallsParent, "Wall_Right", matWallTranslucent, true,
                new Vector3(xMax + wallT * 0.5f, 0, midZ), fullH + wallT * 2, wallH, wallT, 2);

            // ── Interior walls with door frames ──
            float interiorZ = 2.5f * cs;
            float interiorX = 2.5f * cs;
            float doorGap = cs * 1.1f;

            float hSegLen = (midX - doorGap * 0.5f) - xMin;
            MakeWallSegment(wallsParent, "IWall_H_Left", matWall,
                new Vector3(xMin + hSegLen * 0.5f, wallH * 0.5f, interiorZ),
                new Vector3(hSegLen, wallH, wallT));
            BuildDoorFrame(wallsParent, "Door_H",
                new Vector3(midX, 0, interiorZ), doorGap, wallH, wallT, false);
            MakeWallSegment(wallsParent, "IWall_H_Right", matWall,
                new Vector3(xMax - hSegLen * 0.5f, wallH * 0.5f, interiorZ),
                new Vector3(hSegLen, wallH, wallT));

            float vSegLen = (midZ - doorGap * 0.5f) - zMin;
            MakeWallSegment(wallsParent, "IWall_V_Bottom", matWall,
                new Vector3(interiorX, wallH * 0.5f, zMin + vSegLen * 0.5f),
                new Vector3(wallT, wallH, vSegLen));
            BuildDoorFrame(wallsParent, "Door_V",
                new Vector3(interiorX, 0, midZ), doorGap, wallH, wallT, true);
            MakeWallSegment(wallsParent, "IWall_V_Top", matWall,
                new Vector3(interiorX, wallH * 0.5f, zMax - vSegLen * 0.5f),
                new Vector3(wallT, wallH, vSegLen));

            AddTrimToAllChildren(wallsParent);

            CreateRoomLabel(board, "SALÓN", new Vector3(0.75f * cs, 0.05f, 3.75f * cs));
            CreateRoomLabel(board, "DORMITORIO", new Vector3(3.75f * cs, 0.05f, 3.75f * cs));
            CreateRoomLabel(board, "HAB. INVITADOS", new Vector3(0.75f * cs, 0.05f, 0.75f * cs));
            CreateRoomLabel(board, "COMEDOR", new Vector3(3.75f * cs, 0.05f, 0.75f * cs));

            GameObject furnitureParent = new GameObject("FixedFurniture");
            furnitureParent.transform.SetParent(board.transform);

            CreateSofa(furnitureParent, gm, 0, 4);
            CreateRug(furnitureParent, gm, 1, 4);
            CreatePlant(furnitureParent, gm, 2, 5);
            CreateBed(furnitureParent, gm, 4, 5);
            CreateLamp(furnitureParent, gm, 3, 5);
            CreateBed(furnitureParent, gm, 0, 0);
            CreatePlant(furnitureParent, gm, 2, 1);
            CreateTable(furnitureParent, gm, 4, 1);
            CreateChair(furnitureParent, gm, 3, 1);
            CreateChair(furnitureParent, gm, 5, 1);

            if (includeSuspects)
            {
                GameObject suspectsParent = new GameObject("DraggablePieces");
                suspectsParent.transform.SetParent(board.transform);
                CreateAxel(suspectsParent, gm, 1, 2);
                CreateBella(suspectsParent, gm, 2, 2);
                CreateCora(suspectsParent, gm, 4, 3);
                CreateDouglas(suspectsParent, gm, 5, 4);
            }

            return gridObj;
        }

        // ═══════════════════════════════════════════════════════════════
        // WALL SYSTEM — Segments, Windows, Doors
        // ═══════════════════════════════════════════════════════════════
        private void MakeWallSegment(GameObject parent, string name, Material mat, Vector3 pos, Vector3 scale)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent.transform);
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().sharedMaterial = mat;
        }

        // Exterior wall with window cutouts
        private void BuildExteriorWallWithWindows(GameObject parent, string name, Material wallMat, bool isVertical,
            Vector3 basePos, float length, float height, float thickness, int windowCount)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent.transform);
            root.transform.position = basePos;

            float winW = length * 0.18f;   // window width
            float winH = height * 0.45f;   // window height
            float winBottom = height * 0.35f; // window sill height
            float belowH = winBottom;       // wall below window
            float aboveH = height - winBottom - winH; // wall above window
            float segSpacing = length / (windowCount + 1);

            // Build wall as segments around windows
            float cursor = -length * 0.5f;
            for (int i = 0; i < windowCount; i++)
            {
                float winCenter = -length * 0.5f + segSpacing * (i + 1);
                float segEnd = winCenter - winW * 0.5f;
                float segLen = segEnd - cursor;

                if (segLen > 0.05f)
                {
                    Vector3 segPos, segScale;
                    if (isVertical)
                    {
                        segPos = new Vector3(0, height * 0.5f, cursor + segLen * 0.5f);
                        segScale = new Vector3(thickness, height, segLen);
                    }
                    else
                    {
                        segPos = new Vector3(cursor + segLen * 0.5f, height * 0.5f, 0);
                        segScale = new Vector3(segLen, height, thickness);
                    }
                    Prim(root, $"Seg_{i}a", PrimitiveType.Cube, segPos, segScale, wallMat);
                }

                // Wall below window
                Vector3 belowPos, belowScale;
                if (isVertical)
                {
                    belowPos = new Vector3(0, belowH * 0.5f, winCenter);
                    belowScale = new Vector3(thickness, belowH, winW);
                }
                else
                {
                    belowPos = new Vector3(winCenter, belowH * 0.5f, 0);
                    belowScale = new Vector3(winW, belowH, thickness);
                }
                Prim(root, $"Below_{i}", PrimitiveType.Cube, belowPos, belowScale, wallMat);

                // Wall above window
                if (aboveH > 0.02f)
                {
                    Vector3 abovePos;
                    Vector3 aboveScale;
                    if (isVertical)
                    {
                        abovePos = new Vector3(0, winBottom + winH + aboveH * 0.5f, winCenter);
                        aboveScale = new Vector3(thickness, aboveH, winW);
                    }
                    else
                    {
                        abovePos = new Vector3(winCenter, winBottom + winH + aboveH * 0.5f, 0);
                        aboveScale = new Vector3(winW, aboveH, thickness);
                    }
                    Prim(root, $"Above_{i}", PrimitiveType.Cube, abovePos, aboveScale, wallMat);
                }

                // Window frame (4 thin bars)
                float frameT = 0.03f;
                Vector3 wfCenter;
                if (isVertical)
                    wfCenter = new Vector3(0, winBottom + winH * 0.5f, winCenter);
                else
                    wfCenter = new Vector3(winCenter, winBottom + winH * 0.5f, 0);

                // Horizontal bars
                if (isVertical)
                {
                    Prim(root, $"WF_Top_{i}",    PrimitiveType.Cube, wfCenter + new Vector3(0, winH*0.5f, 0), new Vector3(thickness*1.3f, frameT, winW+frameT*2), matWindowFrame);
                    Prim(root, $"WF_Bot_{i}",    PrimitiveType.Cube, wfCenter - new Vector3(0, winH*0.5f, 0), new Vector3(thickness*1.3f, frameT, winW+frameT*2), matWindowFrame);
                    Prim(root, $"WF_Left_{i}",   PrimitiveType.Cube, wfCenter + new Vector3(0, 0, -winW*0.5f), new Vector3(thickness*1.3f, winH, frameT), matWindowFrame);
                    Prim(root, $"WF_Right_{i}",  PrimitiveType.Cube, wfCenter + new Vector3(0, 0, winW*0.5f),  new Vector3(thickness*1.3f, winH, frameT), matWindowFrame);
                    Prim(root, $"WF_Cross_{i}",  PrimitiveType.Cube, wfCenter, new Vector3(thickness*1.3f, frameT*0.8f, winW), matWindowFrame);
                    // Glass pane
                    Prim(root, $"Glass_{i}",     PrimitiveType.Cube, wfCenter, new Vector3(thickness*0.5f, winH*0.9f, winW*0.9f), matGlass);
                }
                else
                {
                    Prim(root, $"WF_Top_{i}",    PrimitiveType.Cube, wfCenter + new Vector3(0, winH*0.5f, 0), new Vector3(winW+frameT*2, frameT, thickness*1.3f), matWindowFrame);
                    Prim(root, $"WF_Bot_{i}",    PrimitiveType.Cube, wfCenter - new Vector3(0, winH*0.5f, 0), new Vector3(winW+frameT*2, frameT, thickness*1.3f), matWindowFrame);
                    Prim(root, $"WF_Left_{i}",   PrimitiveType.Cube, wfCenter + new Vector3(-winW*0.5f, 0, 0), new Vector3(frameT, winH, thickness*1.3f), matWindowFrame);
                    Prim(root, $"WF_Right_{i}",  PrimitiveType.Cube, wfCenter + new Vector3(winW*0.5f, 0, 0),  new Vector3(frameT, winH, thickness*1.3f), matWindowFrame);
                    Prim(root, $"WF_Cross_{i}",  PrimitiveType.Cube, wfCenter, new Vector3(winW, frameT*0.8f, thickness*1.3f), matWindowFrame);
                    // Glass pane
                    Prim(root, $"Glass_{i}",     PrimitiveType.Cube, wfCenter, new Vector3(winW*0.9f, winH*0.9f, thickness*0.5f), matGlass);
                }

                // Window sill
                Vector3 sillPos;
                Vector3 sillScale;
                if (isVertical)
                {
                    sillPos = new Vector3(thickness * 0.6f, winBottom - 0.01f, winCenter);
                    sillScale = new Vector3(thickness * 2.5f, 0.03f, winW + 0.08f);
                }
                else
                {
                    sillPos = new Vector3(winCenter, winBottom - 0.01f, thickness * 0.6f);
                    sillScale = new Vector3(winW + 0.08f, 0.03f, thickness * 2.5f);
                }
                Prim(root, $"Sill_{i}", PrimitiveType.Cube, sillPos, sillScale, matWindowFrame);

                cursor = winCenter + winW * 0.5f;
            }

            // Final segment after last window
            float remainLen = length * 0.5f - cursor;
            if (remainLen > 0.05f)
            {
                Vector3 fPos, fScale;
                if (isVertical)
                {
                    fPos = new Vector3(0, height * 0.5f, cursor + remainLen * 0.5f);
                    fScale = new Vector3(thickness, height, remainLen);
                }
                else
                {
                    fPos = new Vector3(cursor + remainLen * 0.5f, height * 0.5f, 0);
                    fScale = new Vector3(remainLen, height, thickness);
                }
                Prim(root, "Seg_final", PrimitiveType.Cube, fPos, fScale, wallMat);
            }
        }

        // Door frame with lintel and posts
        private void BuildDoorFrame(GameObject parent, string name, Vector3 basePos, float gapW, float height, float thickness, bool isVertical)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent.transform);
            root.transform.position = basePos;

            float postW = 0.06f;
            float lintelH = 0.08f;
            float doorH = height * 0.85f;

            if (isVertical)
            {
                // Two posts
                Prim(root, "Post_L", PrimitiveType.Cube, new Vector3(0, doorH*0.5f, -gapW*0.5f), new Vector3(thickness*1.5f, doorH, postW), matDoorFrame);
                Prim(root, "Post_R", PrimitiveType.Cube, new Vector3(0, doorH*0.5f,  gapW*0.5f), new Vector3(thickness*1.5f, doorH, postW), matDoorFrame);
                // Lintel
                Prim(root, "Lintel", PrimitiveType.Cube, new Vector3(0, doorH, 0), new Vector3(thickness*1.5f, lintelH, gapW+postW*2), matDoorFrame);
                // Door panel (slightly ajar — rotated 20°)
                var door = Prim(root, "DoorPanel", PrimitiveType.Cube, new Vector3(thickness*0.3f, doorH*0.45f, -gapW*0.25f), new Vector3(0.04f, doorH*0.85f, gapW*0.45f), matDoor);
                door.transform.localRotation = Quaternion.Euler(0, 20f, 0);
                // Door handle
                Prim(root, "Handle", PrimitiveType.Sphere, new Vector3(thickness*0.45f, doorH*0.42f, -gapW*0.05f), new Vector3(0.04f, 0.04f, 0.04f), matTrim);
            }
            else
            {
                Prim(root, "Post_L", PrimitiveType.Cube, new Vector3(-gapW*0.5f, doorH*0.5f, 0), new Vector3(postW, doorH, thickness*1.5f), matDoorFrame);
                Prim(root, "Post_R", PrimitiveType.Cube, new Vector3( gapW*0.5f, doorH*0.5f, 0), new Vector3(postW, doorH, thickness*1.5f), matDoorFrame);
                Prim(root, "Lintel", PrimitiveType.Cube, new Vector3(0, doorH, 0), new Vector3(gapW+postW*2, lintelH, thickness*1.5f), matDoorFrame);
                var door = Prim(root, "DoorPanel", PrimitiveType.Cube, new Vector3(-gapW*0.25f, doorH*0.45f, thickness*0.3f), new Vector3(gapW*0.45f, doorH*0.85f, 0.04f), matDoor);
                door.transform.localRotation = Quaternion.Euler(0, 20f, 0);
                Prim(root, "Handle", PrimitiveType.Sphere, new Vector3(-gapW*0.05f, doorH*0.42f, thickness*0.45f), new Vector3(0.04f, 0.04f, 0.04f), matTrim);
            }
        }

        private void AddTrimToAllChildren(GameObject wallsParent)
        {
            // Iterate a snapshot to avoid modifying during iteration
            Transform[] children = new Transform[wallsParent.transform.childCount];
            for (int i = 0; i < wallsParent.transform.childCount; i++)
                children[i] = wallsParent.transform.GetChild(i);

            foreach (var child in children)
            {
                if (child.name.StartsWith("Door")) continue; // doors have their own lintel
                // Find the topmost point
                Renderer r = child.GetComponent<Renderer>();
                if (r == null) continue;
                float topY = child.position.y + child.localScale.y * 0.5f;
                Prim(child.gameObject, "Trim", PrimitiveType.Cube,
                    new Vector3(0, 0.52f, 0),
                    new Vector3(1.03f, 0.04f, 1.3f), matTrim);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ROOM LABEL (TextMesh on the floor)
        // ═══════════════════════════════════════════════════════════════
        private void CreateRoomLabel(GameObject parent, string text, Vector3 pos)
        {
            var obj = new GameObject("Label_" + text);
            obj.transform.SetParent(parent.transform);
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.Euler(90, 45, 0); // Flat on floor, angled for iso
            var tm = obj.AddComponent<TextMesh>();
            tm.text = text;
            tm.fontSize = 28;
            tm.characterSize = 0.12f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = new Color(1f, 1f, 1f, 0.15f); // Very subtle
            tm.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var mr = obj.GetComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
        }

        // ═══════════════════════════════════════════════════════════════
        // FURNITURE BUILDERS (Fixed, non-draggable)
        // ═══════════════════════════════════════════════════════════════

        // ─── SOFA (Chesterfield) ───────────────────────────────────────
        private void CreateSofa(GameObject parent, Nigma.Core.GridManager gm, int gx, int gy)
        {
            var root = new GameObject("Sofa");
            root.transform.SetParent(parent.transform);
            root.transform.position = gm.GetWorldPosition(gx, gy);


            Mesh roundedBox = GenerateRoundedBoxMesh(new Vector3(1f, 1f, 1f), 0.15f);

            var seat = new GameObject("Seat");
            seat.transform.SetParent(root.transform);
            seat.transform.localPosition = new Vector3(0, 0.22f, 0);
            seat.transform.localScale = new Vector3(1f, 0.18f, 0.5f);
            seat.AddComponent<MeshFilter>().sharedMesh = roundedBox;
            seat.AddComponent<MeshRenderer>().sharedMaterial = matSofa;

            var back = new GameObject("Back");
            back.transform.SetParent(root.transform);
            back.transform.localPosition = new Vector3(0, 0.48f, -0.2f);
            back.transform.localScale = new Vector3(1f, 0.45f, 0.1f);
            back.AddComponent<MeshFilter>().sharedMesh = roundedBox;
            back.AddComponent<MeshRenderer>().sharedMaterial = matSofa;

            var armL = new GameObject("ArmL");
            armL.transform.SetParent(root.transform);
            armL.transform.localPosition = new Vector3(-0.47f, 0.36f, 0);
            armL.transform.localScale = new Vector3(0.09f, 0.3f, 0.5f);
            armL.AddComponent<MeshFilter>().sharedMesh = roundedBox;
            armL.AddComponent<MeshRenderer>().sharedMaterial = matSofa;

            var armR = new GameObject("ArmR");
            armR.transform.SetParent(root.transform);
            armR.transform.localPosition = new Vector3(0.47f, 0.36f, 0);
            armR.transform.localScale = new Vector3(0.09f, 0.3f, 0.5f);
            armR.AddComponent<MeshFilter>().sharedMesh = roundedBox;
            armR.AddComponent<MeshRenderer>().sharedMaterial = matSofa;
        }

        // ─── BED ───────────────────────────────────────────────────────
        private void CreateBed(GameObject parent, Nigma.Core.GridManager gm, int gx, int gy)
        {
            var root = new GameObject("Bed");
            root.transform.SetParent(parent.transform);
            root.transform.position = gm.GetWorldPosition(gx, gy);


            Mesh roundedBox = GenerateRoundedBoxMesh(new Vector3(1f, 1f, 1f), 0.08f);

            // Frame
            Prim(root, "Frame", PrimitiveType.Cube, new Vector3(0, 0.15f, 0), new Vector3(0.8f, 0.15f, 1.2f), matTable);
            
            // Mattress (Smooth)
            var mattress = new GameObject("Mattress");
            mattress.transform.SetParent(root.transform);
            mattress.transform.localPosition = new Vector3(0, 0.28f, 0);
            mattress.transform.localScale = new Vector3(0.7f, 0.1f, 1.1f);
            mattress.AddComponent<MeshFilter>().sharedMesh = roundedBox;
            mattress.AddComponent<MeshRenderer>().sharedMaterial = matBed;

            // Pillow (Smooth)
            var pillow = new GameObject("Pillow");
            pillow.transform.SetParent(root.transform);
            pillow.transform.localPosition = new Vector3(0, 0.35f, -0.4f);
            pillow.transform.localScale = new Vector3(0.5f, 0.08f, 0.2f);
            pillow.AddComponent<MeshFilter>().sharedMesh = roundedBox;
            pillow.AddComponent<MeshRenderer>().sharedMaterial = MakeMat(new Color(0.92f, 0.9f, 0.85f), 0.2f);
            
            // Headboard
            Prim(root, "Headboard", PrimitiveType.Cube, new Vector3(0, 0.42f, -0.55f), new Vector3(0.8f, 0.4f, 0.06f), matTable);
        }

        // ─── TABLE ─────────────────────────────────────────────────────
        private void CreateTable(GameObject parent, Nigma.Core.GridManager gm, int gx, int gy)
        {
            var root = new GameObject("Table");
            root.transform.SetParent(parent.transform);
            root.transform.position = gm.GetWorldPosition(gx, gy);


            Prim(root, "Top", PrimitiveType.Cube, new Vector3(0, 0.42f, 0), new Vector3(0.9f, 0.06f, 0.6f), matTable);
            // 4 legs
            float lx = 0.35f, lz = 0.22f;
            Prim(root, "Leg1", PrimitiveType.Cylinder, new Vector3(-lx, 0.2f, lz),  new Vector3(0.05f, 0.2f, 0.05f), matTable);
            Prim(root, "Leg2", PrimitiveType.Cylinder, new Vector3(lx, 0.2f, lz),   new Vector3(0.05f, 0.2f, 0.05f), matTable);
            Prim(root, "Leg3", PrimitiveType.Cylinder, new Vector3(-lx, 0.2f, -lz), new Vector3(0.05f, 0.2f, 0.05f), matTable);
            Prim(root, "Leg4", PrimitiveType.Cylinder, new Vector3(lx, 0.2f, -lz),  new Vector3(0.05f, 0.2f, 0.05f), matTable);
        }

        // ─── CHAIR ─────────────────────────────────────────────────────
        private void CreateChair(GameObject parent, Nigma.Core.GridManager gm, int gx, int gy)
        {
            var root = new GameObject("Chair");
            root.transform.SetParent(parent.transform);
            root.transform.position = gm.GetWorldPosition(gx, gy);


            Prim(root, "Seat", PrimitiveType.Cube, new Vector3(0, 0.3f, 0), new Vector3(0.4f, 0.05f, 0.4f), matChair);
            Prim(root, "Back", PrimitiveType.Cube, new Vector3(0, 0.52f, -0.17f), new Vector3(0.4f, 0.4f, 0.05f), matChair);
            Prim(root, "L1", PrimitiveType.Cylinder, new Vector3(-0.15f, 0.15f, 0.15f), new Vector3(0.04f, 0.15f, 0.04f), matChair);
            Prim(root, "L2", PrimitiveType.Cylinder, new Vector3(0.15f, 0.15f, 0.15f),  new Vector3(0.04f, 0.15f, 0.04f), matChair);
            Prim(root, "L3", PrimitiveType.Cylinder, new Vector3(-0.15f, 0.15f, -0.15f),new Vector3(0.04f, 0.15f, 0.04f), matChair);
            Prim(root, "L4", PrimitiveType.Cylinder, new Vector3(0.15f, 0.15f, -0.15f), new Vector3(0.04f, 0.15f, 0.04f), matChair);
        }

        // ─── PLANT ─────────────────────────────────────────────────────
        private void CreatePlant(GameObject parent, Nigma.Core.GridManager gm, int gx, int gy)
        {
            var root = new GameObject("Plant");
            root.transform.SetParent(parent.transform);
            root.transform.position = gm.GetWorldPosition(gx, gy);


            // Pot
            Prim(root, "Pot", PrimitiveType.Cylinder, new Vector3(0, 0.12f, 0), new Vector3(0.3f, 0.12f, 0.3f), matPlantPot);
            // Bush (three overlapping spheres)
            Prim(root, "Leaf1", PrimitiveType.Sphere, new Vector3(0, 0.42f, 0),     new Vector3(0.4f, 0.35f, 0.4f), matPlant);
            Prim(root, "Leaf2", PrimitiveType.Sphere, new Vector3(0.12f, 0.5f, 0.08f), new Vector3(0.3f, 0.28f, 0.3f), matPlant);
            Prim(root, "Leaf3", PrimitiveType.Sphere, new Vector3(-0.1f, 0.48f, -0.06f),new Vector3(0.28f, 0.25f, 0.28f), matPlant);
        }

        // ─── RUG (flat on the floor) ──────────────────────────────────
        private void CreateRug(GameObject parent, Nigma.Core.GridManager gm, int gx, int gy)
        {
            var root = new GameObject("Rug");
            root.transform.SetParent(parent.transform);
            root.transform.position = gm.GetWorldPosition(gx, gy);


            Prim(root, "Rug", PrimitiveType.Cube, new Vector3(0, 0.02f, 0), new Vector3(1.2f, 0.03f, 0.8f), matRug);
            // Fringe detail — slightly lighter border
            Prim(root, "Border", PrimitiveType.Cube, new Vector3(0, 0.015f, 0), new Vector3(1.3f, 0.02f, 0.9f),
                MakeMat(new Color(0.7f, 0.2f, 0.15f), 0.15f));
        }

        // ─── LAMP ──────────────────────────────────────────────────────
        private void CreateLamp(GameObject parent, Nigma.Core.GridManager gm, int gx, int gy)
        {
            var root = new GameObject("Lamp");
            root.transform.SetParent(parent.transform);
            root.transform.position = gm.GetWorldPosition(gx, gy);


            // Base
            Prim(root, "Base", PrimitiveType.Cylinder, new Vector3(0, 0.03f, 0), new Vector3(0.2f, 0.03f, 0.2f), matLamp);
            // Pole
            Prim(root, "Pole", PrimitiveType.Cylinder, new Vector3(0, 0.45f, 0), new Vector3(0.04f, 0.4f, 0.04f), matLamp);
            // Shade
            Prim(root, "Shade", PrimitiveType.Cylinder, new Vector3(0, 0.82f, 0), new Vector3(0.25f, 0.12f, 0.25f), matLampShade);

            // Point light for atmosphere
            var lightObj = new GameObject("LampLight");
            lightObj.transform.SetParent(root.transform);
            lightObj.transform.localPosition = new Vector3(0, 0.7f, 0);
            var pl = lightObj.AddComponent<Light>();
            pl.type = LightType.Point;
            pl.range = 3f;
            pl.intensity = 0.6f;
            pl.color = new Color(1f, 0.9f, 0.7f);
            pl.shadows = LightShadows.Soft;
        }

        // ═══════════════════════════════════════════════════════════════
        // SUSPECT CHARACTERS — Cozy Peg Doll style
        // ═══════════════════════════════════════════════════════════════
        // SUSPECT CHARACTERS — 2D HD Billboards
        // ═══════════════════════════════════════════════════════════════
        private GameObject MakeBillboardChar(GameObject parent, string charName, Nigma.Core.GridManager gm, int gx, int gy)
        {
            var root = new GameObject(charName);
            root.transform.SetParent(parent.transform);
            root.transform.position = gm.GetWorldPosition(gx, gy);

            // Create the Sprite visual
            var visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform);
            
            // A character sprite is likely large, we scale it down to fit 1x1 tile approx
            visual.transform.localPosition = new Vector3(0, 0.45f, 0); 
            visual.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            
            // JUICE: Animación de respiración y pop-in


            var sr = visual.AddComponent<SpriteRenderer>();
            
            // Load the sprite processed by our SpriteImporter
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/Sprites/Characters/{charName}_sprite.png");
            if (sp != null)
            {
                sr.sprite = sp;
            }
            else
            {
                Debug.LogWarning($"[PlayableSceneGenerator] Missing sprite for {charName}. Please run 'Nigma -> Herramientas -> Procesar Sprites 2D' first.");
                // Fallback to primitive
                var fallback = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                fallback.transform.SetParent(visual.transform);
                fallback.transform.localPosition = Vector3.zero;
                fallback.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            }

            // Attach billboard script to face the camera
            visual.AddComponent<Nigma.Core.Billboard>();

            // Physics and Drag logic
            BoxCollider col = root.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 0.4f, 0);
            col.size = new Vector3(0.4f, 0.8f, 0.4f);
            
            var drag = root.AddComponent<Nigma.Core.DraggableObject>();
            drag.furnitureType = Nigma.Core.FurnitureType.Character;
            drag.liftHeight = 0.8f;
            return root;
        }

        private void CreateAxel(GameObject parent, Nigma.Core.GridManager gm, int gx, int gy)
        {
            MakeBillboardChar(parent, "Axel", gm, gx, gy);
        }

        private void CreateBella(GameObject parent, Nigma.Core.GridManager gm, int gx, int gy)
        {
            MakeBillboardChar(parent, "Bella", gm, gx, gy);
        }

        private void CreateCora(GameObject parent, Nigma.Core.GridManager gm, int gx, int gy)
        {
            MakeBillboardChar(parent, "Cora", gm, gx, gy);
        }

        private void CreateDouglas(GameObject parent, Nigma.Core.GridManager gm, int gx, int gy)
        {
            MakeBillboardChar(parent, "Douglas", gm, gx, gy);
        }


        // ═══════════════════════════════════════════════════════════════
        // PROCEDURAL MESH GENERATION (Cozy Art Style)
        // ═══════════════════════════════════════════════════════════════
        private Mesh GeneratePegDollMesh()
        {
            // Lathe profile for a Peg Doll / Pawn
            Vector2[] profile = new Vector2[]
            {
                new Vector2(0f, 0f),      // Bottom center
                new Vector2(0.35f, 0f),   // Bottom edge
                new Vector2(0.33f, 0.15f),
                new Vector2(0.28f, 0.35f),
                new Vector2(0.22f, 0.5f),
                new Vector2(0.1f, 0.62f), // Neck
                new Vector2(0.25f, 0.7f), // Chin
                new Vector2(0.3f, 0.85f), // Head middle
                new Vector2(0.25f, 0.98f),
                new Vector2(0.15f, 1.05f),
                new Vector2(0f, 1.08f)    // Top center
            };

            int segments = 24; 
            System.Collections.Generic.List<Vector3> verts = new System.Collections.Generic.List<Vector3>();
            System.Collections.Generic.List<int> tris = new System.Collections.Generic.List<int>();

            for (int p = 0; p < profile.Length; p++)
            {
                float radius = profile[p].x;
                float height = profile[p].y;
                for (int s = 0; s <= segments; s++)
                {
                    float angle = (float)s / segments * Mathf.PI * 2f;
                    verts.Add(new Vector3(Mathf.Sin(angle) * radius, height, Mathf.Cos(angle) * radius));
                }
            }

            for (int p = 0; p < profile.Length - 1; p++)
            {
                for (int s = 0; s < segments; s++)
                {
                    int current = p * (segments + 1) + s;
                    int next = current + segments + 1;
                    tris.Add(current); tris.Add(next + 1); tris.Add(next);
                    tris.Add(current); tris.Add(current + 1); tris.Add(next + 1);
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "PegDoll";
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            return mesh;
        }

        private Mesh GenerateRoundedBoxMesh(Vector3 size, float radius)
        {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Mesh sphere = temp.GetComponent<MeshFilter>().sharedMesh;
            Vector3[] verts = sphere.vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 v = verts[i];
                float hx = Mathf.Max(0, (size.x * 0.5f) - radius);
                float hy = Mathf.Max(0, (size.y * 0.5f) - radius);
                float hz = Mathf.Max(0, (size.z * 0.5f) - radius);
                v *= radius; 
                v.x += (verts[i].x > 0 ? hx : -hx);
                v.y += (verts[i].y > 0 ? hy : -hy);
                v.z += (verts[i].z > 0 ? hz : -hz);
                verts[i] = v;
            }
            Mesh box = new Mesh();
            box.name = "RoundedBox";
            box.vertices = verts;
            box.triangles = sphere.triangles;
            box.normals = sphere.normals;
            box.uv = sphere.uv;
            box.RecalculateNormals();
            DestroyImmediate(temp);
            return box;
        }

        // ═══════════════════════════════════════════════════════════════
        // UI
        // ═══════════════════════════════════════════════════════════════
        private void BuildUI(Camera cam, Nigma.Core.GridManager gm)
        {
            var canvasObj = new GameObject("Canvas_Gameplay");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920); // Portrait Mobile
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            // ── Atestado panel (top, full width) ──
            var panel = MakeUIPanel(canvas, "Panel_Atestado",
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
                new Vector2(0, -20), new Vector2(-40, 150),
                new Color(0.12f, 0.08f, 0.04f, 0.95f));

            // Icon
            MakeUIText(panel, "Icon", "🔍", 40, TextAnchor.MiddleCenter,
                new Color(0.9f, 0.75f, 0.35f),
                new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f),
                new Vector2(20, 0), new Vector2(100, 0));

            // Atestado text
            var txtAtestado = MakeUIText(panel, "Txt_Atestado",
                "El asesino estaba solo en una habitación con la víctima.\nColocad a los sospechosos y pulsad Resolver.",
                28, TextAnchor.MiddleLeft, new Color(0.92f, 0.87f, 0.72f),
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f),
                new Vector2(120, 10), new Vector2(-20, -10));
            txtAtestado.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;

            // ── Suspects panel (bottom left) ────────────────────
            var suspectsPanel = MakeUIPanel(canvas, "Panel_Suspects",
                new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                new Vector2(20, 250), new Vector2(450, 500),
                new Color(0.12f, 0.08f, 0.04f, 0.95f));

            MakeUIText(suspectsPanel, "Title", "PISTAS SOSPECHOSOS", 24, TextAnchor.MiddleCenter,
                new Color(0.9f, 0.75f, 0.35f),
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
                new Vector2(10, -10), new Vector2(-10, -50));

            string[] names = { "Axel (Azul)", "Bella (Rojo)", "Cora (Verde)", "Douglas (Morado)" };
            string[] hints = {
                "  Estaba al lado de\n  una ventana.",
                "  Estaba en la Hab.\n  de Invitados.",
                "  Estaba de pie sobre\n  una alfombra.",
                "  Yo era la única\n  persona en una cama."
            };
            Color[] colors = {
                new Color(0.4f, 0.65f, 0.9f),
                new Color(0.9f, 0.4f, 0.4f),
                new Color(0.4f, 0.8f, 0.5f),
                new Color(0.75f, 0.55f, 0.85f)
            };
            for (int i = 0; i < 4; i++)
            {
                float yOff = -60 - i * 110;
                MakeUIText(suspectsPanel, $"Suspect_{i}", names[i] + "\n" + hints[i],
                    20, TextAnchor.UpperLeft, colors[i],
                    new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
                    new Vector2(20, yOff - 100), new Vector2(-10, yOff));
            }

            // ── Room names legend (bottom left, under suspects) ──────────────────────────
            var legendPanel = MakeUIPanel(canvas, "Panel_Legend",
                new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                new Vector2(20, 20), new Vector2(450, 200),
                new Color(0.12f, 0.08f, 0.04f, 0.9f));
            MakeUIText(legendPanel, "Legend", "Silla  -  Cama  -  Alfombra\nPlanta  -  Lámpara  -  Mesa", 22, TextAnchor.MiddleCenter,
                new Color(0.8f, 0.75f, 0.6f),
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                new Vector2(10, 10), new Vector2(-10, -10));

            // ── Resolver button (bottom-right, massive) ──────────────────────────
            var btnObj = new GameObject("Btn_Resolver");
            btnObj.transform.SetParent(canvas.transform, false);
            Button btnResolve = btnObj.AddComponent<Button>();
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.6f, 0.45f, 0.15f);
            RectTransform btnRt = btnObj.GetComponent<RectTransform>();
            btnRt.anchorMin = new Vector2(1, 0);
            btnRt.anchorMax = new Vector2(1, 0);
            btnRt.pivot = new Vector2(1, 0);
            btnRt.anchoredPosition = new Vector2(-20, 20);
            btnRt.sizeDelta = new Vector2(450, 180); // MASSIVE for mobile

            ColorBlock cb = btnResolve.colors;
            cb.normalColor = new Color(0.6f, 0.45f, 0.15f);
            cb.highlightedColor = new Color(0.75f, 0.58f, 0.25f);
            cb.pressedColor = new Color(0.45f, 0.33f, 0.1f);
            btnResolve.colors = cb;

            var btnTxt = MakeUIText(btnObj, "Text", "RESOLVER", 48, TextAnchor.MiddleCenter,
                new Color(0.95f, 0.95f, 0.9f),
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, Vector2.zero);

            btnTxt.fontStyle = FontStyle.Bold;

            // ── Victory overlay ─────────────────────────────────────────
            var vicPanel = MakeUIPanel(canvas, "Panel_Victory",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(500, 120),
                new Color(0.05f, 0.04f, 0.02f, 0.95f));

            Outline vo = vicPanel.AddComponent<Outline>();
            vo.effectColor = new Color(0.85f, 0.7f, 0.2f);
            vo.effectDistance = new Vector2(3, -3);

            var txtVic = MakeUIText(vicPanel, "Txt_Victory", "¡CASO RESUELTO!", 38, TextAnchor.MiddleCenter,
                new Color(0.95f, 0.85f, 0.3f),
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                new Vector2(10, 10), new Vector2(-10, -10));
            txtVic.fontStyle = FontStyle.Bold;
            vicPanel.SetActive(false);

            // ── Wire Game Logic ─────────────────────────────────────────
            if (!Directory.Exists("Assets/Resources")) Directory.CreateDirectory("Assets/Resources");

            var level = ScriptableObject.CreateInstance<Nigma.Core.LevelData>();
            level.levelID = 1;
            level.puzzleDescription = "El asesino estaba solo en una habitación con la víctima.";
            level.correctAnswer = new Vector2Int(4, 4);
            AssetDatabase.CreateAsset(level, "Assets/Resources/DummyLevel1.asset");
            AssetDatabase.SaveAssets();

            var uiMgr = FindObjectOfType<Nigma.Core.UIManager>();
            if (uiMgr == null)
            {
                var uiObj = new GameObject("UIManager");
                uiMgr = uiObj.AddComponent<Nigma.Core.UIManager>();
            }

            var gameMgr = FindObjectOfType<Nigma.Core.GameManager>();
            if (gameMgr != null) DestroyImmediate(gameMgr.gameObject);
            var gmObj = new GameObject("GameManager");
            gameMgr = gmObj.AddComponent<Nigma.Core.GameManager>();
            gameMgr.levels.Add(level);

            // JUICE: Añadir Post-Processing y Audio
            gmObj.AddComponent<Nigma.Core.CozyPostProcessing>();
            if (UnityEngine.Object.FindObjectOfType<Nigma.Core.AudioManager>() == null)
            {
                var audioMgr = new GameObject("AudioManager");
                audioMgr.AddComponent<Nigma.Core.AudioManager>();
            }

            UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(
                btnResolve.onClick,
                new UnityEngine.Events.UnityAction(gameMgr.OnSolveButtonClicked));

            // Reflection wiring
            System.Reflection.BindingFlags bf = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            typeof(Nigma.Core.UIManager).GetField("txtPuzzle", bf)?.SetValue(uiMgr, txtAtestado);
            typeof(Nigma.Core.UIManager).GetField("txtFeedback", bf)?.SetValue(uiMgr, txtVic);
            typeof(Nigma.Core.UIManager).GetField("panelVictory", bf)?.SetValue(uiMgr, vicPanel);
            gameMgr.uiManager = uiMgr;
        }

        // ═══════════════════════════════════════════════════════════════
        // UI HELPERS
        // ═══════════════════════════════════════════════════════════════
        private GameObject MakeUIPanel(Canvas canvas, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPos, Vector2 sizeDelta, Color bgColor)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(canvas.transform, false);
            var img = obj.AddComponent<Image>();
            img.color = bgColor;
            var rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            var outline = obj.AddComponent<Outline>();
            outline.effectColor = new Color(0.72f, 0.56f, 0.28f, 0.7f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            return obj;
        }

        private Text MakeUIText(GameObject parent, string name, string content,
            int fontSize, TextAnchor align, Color color,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent.transform, false);
            var txt = obj.AddComponent<Text>();
            txt.text = content;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = fontSize;
            txt.color = color;
            txt.alignment = align;
            txt.horizontalOverflow = HorizontalWrapMode.Wrap;
            txt.verticalOverflow = VerticalWrapMode.Overflow;
            var rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            return txt;
        }

        // ═══════════════════════════════════════════════════════════════
        // PRIMITIVE HELPER (create child, strip collider)
        // ═══════════════════════════════════════════════════════════════
        private GameObject Prim(GameObject parent, string name, PrimitiveType type, Vector3 localPos, Vector3 localScale, Material mat)
        {
            var obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.SetParent(parent.transform);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = localScale;
            obj.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(obj.GetComponent<Collider>());
            return obj;
        }
    }
}
