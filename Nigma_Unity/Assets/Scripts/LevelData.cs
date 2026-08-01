using System.Collections.Generic;
using UnityEngine;

namespace Nigma.Core
{
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "Nigma/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Story & Riddle")]
        public int levelID;
        [TextArea(3, 10)]
        public string puzzleDescription;

        [Header("Solution")]
        [Tooltip("The correct grid coordinate (x, y) to solve the level.")]
        public Vector2Int correctAnswer;

        [Header("Inventory (Maletín) — Fase 2/3")]
        [Tooltip("Number of mirrors available for this level.")]
        public int availableMirrors;
        [Tooltip("Number of sofas available for this level.")]
        public int availableSofas;

        [Header("Inventory (Maletín) — Fase 4: New Objects")]
        [Tooltip("Number of surveillance cameras available for this level.")]
        public int availableCameras;
        [Tooltip("Number of floor lamps available for this level (used in dark levels).")]
        public int availableLamps;
        [Tooltip("Number of interior plants available for this level.")]
        public int availablePlants;
        [Tooltip("Number of fans available for this level.")]
        public int availableFans;

        [Header("Level Layout — Structural")]
        [Tooltip("Grid coordinates for pre-placed structural walls.")]
        public List<Vector2Int> structuralWalls = new List<Vector2Int>();
        [Tooltip("Grid coordinates for pre-placed permanent mirrors (if any).")]
        public List<Vector2Int> permanentMirrors = new List<Vector2Int>();

        [Header("Level Layout — Phase 4 Objects")]
        [Tooltip("Grid coordinates for pre-placed surveillance cameras. Cameras face East by default.")]
        public List<Vector2Int> permanentCameras = new List<Vector2Int>();
        [Tooltip("Grid coordinates for pre-placed fans. Fans blow East by default.")]
        public List<Vector2Int> permanentFans = new List<Vector2Int>();

        [Header("Level Rules")]
        [Tooltip("If true, only objects inside a Lamp's light radius are visible. Used for noir/dark levels.")]
        public bool isLightRequired = false;
        [Tooltip("Clue fragments for Policia Corrupto multiplayer mode. Each string is one clue fragment distributed to a player.")]
        [TextArea(2, 5)]
        public List<string> multiplayerClueFragments = new List<string>();

        [Header("Caja Fuerte (Meta-puzzle)")]
        [Tooltip("Código de la caja fuerte. Dejar vacío si el nivel no tiene caja fuerte.")]
        public string safeCode = "";
        [Tooltip("Pista que se muestra al jugador para que deduzca el código del tablero.")]
        [TextArea(2, 4)]
        public string safeHint = "";
    }
}
