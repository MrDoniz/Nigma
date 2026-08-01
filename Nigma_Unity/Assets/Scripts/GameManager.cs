using System.Collections.Generic;
using UnityEngine;

namespace Nigma.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        // ─────────────────────────────────────────────────────────────────────
        #region Fields

        [Header("Level Management")]
        public List<LevelData> levels = new List<LevelData>();
        public int currentLevelIndex = 0;

        [Header("UI & Systems")]
        public UIManager uiManager;
        private GridManager gridManager;
        private JokerManager jokerManager;
        private SafeManager safeManager;

        [Header("Scoring")]
        [Tooltip("Puntuación base que recibe el jugador por resolver un nivel.")]
        public int baseScorePerLevel = 100;
        private int totalScore = 0;

        private bool isResolving = false;
        private LevelData currentLevel;

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            gridManager   = FindObjectOfType<GridManager>();
            jokerManager  = FindObjectOfType<JokerManager>();
            safeManager   = FindObjectOfType<SafeManager>();
            if (uiManager == null) uiManager = UIManager.Instance;

            LoadLevel(currentLevelIndex);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Level Loading

        public void LoadLevel(int index)
        {
            if (index < 0 || index >= levels.Count)
            {
                Debug.LogWarning("[GameManager] Índice de nivel inválido o no hay niveles asignados.");
                return;
            }

            currentLevelIndex = index;
            currentLevel      = levels[currentLevelIndex];
            isResolving       = false;

            // Notificar al JokerManager para reiniciar el temporizador
            jokerManager?.OnLevelLoaded();

            // Limpiar y preparar el grid
            if (gridManager != null)
            {
                gridManager.ClearGrid();
                foreach (var wallPos in currentLevel.structuralWalls)
                    gridManager.SpawnStructuralWall(wallPos.x, wallPos.y);
            }

            // Actualizar la UI
            if (uiManager != null)
            {
                uiManager.UpdatePuzzleText(currentLevel.puzzleDescription);
                uiManager.UpdateInventory(currentLevel.availableMirrors, currentLevel.availableSofas);
                uiManager.ShowVictoryPanel(false);
                uiManager.UpdateFeedbackText("");
            }

            // Abrir Caja Fuerte si el nivel la tiene configurada
            if (!string.IsNullOrEmpty(currentLevel.safeCode) && safeManager != null)
            {
                safeManager.OpenSafe(currentLevel.safeCode, currentLevel.safeHint);
            }

            Debug.Log($"[GameManager] Nivel {currentLevelIndex + 1} cargado: '{currentLevel.puzzleDescription.Split('\n')[0]}'");
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Solve Flow

        public void OnSolveButtonClicked()
        {
            if (currentLevel == null) return;
            isResolving = true;
            Debug.Log("[GameManager] Modo Resolver activo. Esperando clic en el tablero...");
            uiManager?.UpdatePuzzleText("🔍 Señala la casilla correcta en el tablero...");
        }

        private void Update()
        {
            if (!isResolving) return;
            if (UnityEngine.InputSystem.Mouse.current == null) return;
            if (!UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame) return;

            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float rayDistance))
            {
                Vector3 hitPoint = ray.GetPoint(rayDistance);
                GridManager.GridNode clickedNode = gridManager.GetNearestNode(hitPoint);
                CheckVictoryCondition(clickedNode);
            }
        }

        private void CheckVictoryCondition(GridManager.GridNode clickedNode)
        {
            if (currentLevel == null) return;

            bool isCorrect = clickedNode.x == currentLevel.correctAnswer.x
                          && clickedNode.y == currentLevel.correctAnswer.y;

            if (isCorrect)
            {
                isResolving = false;
                OnVictory();
            }
            else
            {
                isResolving = false;
                jokerManager?.OnWrongAnswer();
                Debug.Log("[GameManager] Respuesta incorrecta.");
                uiManager?.UpdatePuzzleText("❌ Incorrecto. Vuelve a leer el atestado:\n\n" + currentLevel.puzzleDescription);
            }
        }

        private void OnVictory()
        {
            // Calcular puntuación con Jokers
            JokerResult result = jokerManager != null
                ? jokerManager.ApplyJokers(baseScorePerLevel)
                : new JokerResult { baseScore = baseScorePerLevel, finalScore = baseScorePerLevel, totalMultiplier = 1f };

            totalScore += result.finalScore;

            // Construir mensaje de victoria con los Jokers activados
            string jokerText = result.triggeredJokerNames != null && result.triggeredJokerNames.Count > 0
                ? "\n✨ Jokers: " + string.Join(", ", result.triggeredJokerNames) + $" (x{result.totalMultiplier})"
                : "";

            string victoryMsg = $"✅ ¡Correcto! Misterio resuelto.\n+{result.finalScore} pts{jokerText}\nTotal: {totalScore} pts";

            Debug.Log($"[GameManager] ¡Victoria! Puntuación: {result.finalScore} (base {result.baseScore} x{result.totalMultiplier}). Total: {totalScore}");

            uiManager?.ShowVictoryPanel(true);
            uiManager?.UpdateFeedbackText(victoryMsg);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Navigation

        public void NextLevel()
        {
            if (currentLevelIndex + 1 < levels.Count)
            {
                LoadLevel(currentLevelIndex + 1);
            }
            else
            {
                Debug.Log("[GameManager] ¡Todos los niveles completados!");
                uiManager?.UpdateFeedbackText($"🏆 ¡Caso cerrado! Puntuación final: {totalScore} pts");
            }
        }

        #endregion
    }
}
