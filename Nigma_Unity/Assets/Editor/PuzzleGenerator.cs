#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Nigma.Core;

/// <summary>
/// Genera los 3 primeros puzzles reales del juego con atestados policiales
/// coherentes con la mecánica de "Líneas de Visión" del GDD.
/// 
/// Ejecutar desde: Nigma → 3. Generar Puzzles Reales
/// </summary>
public class PuzzleGenerator : UnityEditor.Editor
{
    [MenuItem("Nigma/3. Generar Puzzles Reales (Niveles 1-3)")]
    public static void GenerateRealPuzzles()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
            AssetDatabase.CreateFolder("Assets", "Data");

        // ─── NIVEL 1: El Caso de la Tarta Desaparecida ───────────────────────
        // Grid 4x4. Sin muros. 1 sofá, 1 espejo. 
        // Respuesta: El testigo Gato en (1,2) jura que vio desde el sofá pero el sofá
        // bloqueaba su vista → La tarta estaba en (3,1), fuera de la línea de visión.
        {
            string path = "Assets/Data/Level1.asset";
            LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path)
                           ?? ScriptableObject.CreateInstance<LevelData>();

            level.levelID = 1;
            level.puzzleDescription =
                "🔎 CASO 1: «La Tarta Desaparecida»\n\n" +
                "La señora Miau dejó su tarta en la cocina y al volver la encontró a medias.\n" +
                "El Gato jura que estaba en el sofá y vio perfectamente la cocina todo el tiempo.\n" +
                "Pero el Perro insiste que el sofá bloqueaba la vista hacia el lado derecho.\n\n" +
                "¿En qué casilla estaba la tarta cuando desapareció?\n" +
                "(Coloca el Sofá en la casilla central y deduce la línea de visión del Gato)";

            level.correctAnswer        = new Vector2Int(3, 1);
            level.availableMirrors     = 0;
            level.availableSofas       = 1;
            level.structuralWalls      = new List<Vector2Int>();   // Sin muros estructurales
            level.safeCode             = "";                       // Nivel 1 sin Caja Fuerte
            level.safeHint             = "";

            if (AssetDatabase.LoadAssetAtPath<LevelData>(path) == null)
                AssetDatabase.CreateAsset(level, path);
            else
                EditorUtility.SetDirty(level);
        }

        // ─── NIVEL 2: El Robo en la Galería ──────────────────────────────────
        // Grid 4x4. Muro estructural en (1,1). 1 espejo, 1 sofá.
        // El vigilante Búho alega que vio el cuadro desde su puesto.
        // Pero el espejo estaba girado → la línea de visión rebotaba y no alcanzaba.
        // La pieza robada estaba en (2,3) detrás del muro.
        // Caja Fuerte: código "13" (1 persona en esquina, 3 objetos colocados = no coincide
        // → código real calculado por el jugador observando el tablero).
        {
            string path = "Assets/Data/Level2.asset";
            LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path)
                           ?? ScriptableObject.CreateInstance<LevelData>();

            level.levelID = 2;
            level.puzzleDescription =
                "🔎 CASO 2: «El Robo en la Galería»\n\n" +
                "Esta noche han robado el cuadro más valioso del museo.\n" +
                "El vigilante Búho asegura que vigilaba desde el pasillo y tenía línea de visión directa.\n" +
                "La directora Lechuza señala que hay un espejo en la sala que desviaba su vista hacia otra dirección.\n" +
                "El ladrón aprovechó ese ángulo muerto.\n\n" +
                "¿En qué casilla estaba el cuadro cuando lo cogieron?\n" +
                "(El muro gris bloquea el acceso. Usa el Espejo para reconstruir el ángulo del Búho)";

            level.correctAnswer    = new Vector2Int(2, 3);
            level.availableMirrors = 1;
            level.availableSofas   = 1;
            level.structuralWalls  = new List<Vector2Int> { new Vector2Int(1, 1) };
            level.safeCode         = "23";   // El jugador lo deduce: fila + columna de la respuesta
            level.safeHint         =
                "🔒 Para la Caja Fuerte:\n" +
                "\"El código = Coordenada X de la solución seguida de coordenada Y\"\n" +
                "(Dos dígitos, sin espacios)";

            if (AssetDatabase.LoadAssetAtPath<LevelData>(path) == null)
                AssetDatabase.CreateAsset(level, path);
            else
                EditorUtility.SetDirty(level);
        }

        // ─── NIVEL 3: El Testigo Ciego ────────────────────────────────────────
        // Grid 4x4. Muros en (0,2) y (3,2). 2 espejos, 1 sofá.
        // La Zorra dice que vio al sospechoso cruzar por el pasillo central.
        // El Conejo alega que dos muros lo impedían y el espejo estaba mal colocado.
        // La víctima fue vista en (1,3), el único ángulo posible sin muros en la cadena de reflexión.
        {
            string path = "Assets/Data/Level3.asset";
            LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path)
                           ?? ScriptableObject.CreateInstance<LevelData>();

            level.levelID = 3;
            level.puzzleDescription =
                "🔎 CASO 3: «El Testigo Ciego»\n\n" +
                "Un robo con violencia en el piso 3. Hay dos testigos que se contradicen.\n" +
                "La Zorra jura haber visto al culpable cruzar el pasillo central a plena luz.\n" +
                "El Conejo dice que los dos muros laterales lo hacen imposible: hay un ángulo muerto total.\n" +
                "Los espejos en la habitación podían reflejar la visión... o precisamente ocultarla.\n\n" +
                "¿En qué casilla estaba el sospechoso cuando fue (o no fue) visto?\n" +
                "(Coloca los 2 Espejos para reconstruir la cadena de reflexión posible de la Zorra)";

            level.correctAnswer    = new Vector2Int(1, 3);
            level.availableMirrors = 2;
            level.availableSofas   = 1;
            level.structuralWalls  = new List<Vector2Int>
            {
                new Vector2Int(0, 2),
                new Vector2Int(3, 2)
            };
            level.safeCode = "";   // Nivel 3 sin Caja Fuerte; se reserva para futuro
            level.safeHint = "";

            if (AssetDatabase.LoadAssetAtPath<LevelData>(path) == null)
                AssetDatabase.CreateAsset(level, path);
            else
                EditorUtility.SetDirty(level);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ [PuzzleGenerator] 3 puzzles reales generados en Assets/Data/. " +
                  "Abre GameManager en el Inspector y asígnales a la lista 'Levels'.");
    }
}
#endif
