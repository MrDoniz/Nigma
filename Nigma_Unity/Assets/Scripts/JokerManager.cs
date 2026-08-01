using System.Collections.Generic;
using UnityEngine;

namespace Nigma.Core
{
    /// <summary>
    /// Define un Joker pasivo que modifica el meta-juego (puntuación, recompensas).
    /// NO altera las reglas físicas del tablero (diseño bloqueado).
    /// 
    /// Ejemplos del GDD:
    ///  - "Lupa Antigua"  → resuelves en menos de 2 min → puntuación x3
    ///  - "Libreta de Notas" → primera deducción correcta → +50 monedas extra
    /// </summary>
    [System.Serializable]
    public class JokerData
    {
        public string jokerName;
        [TextArea(2, 4)]
        public string description;
        public JokerTrigger trigger;
        public float multiplier = 1f;
        public int bonusCoins = 0;
        public float triggerTimeLimit = 120f; // Segundos (para trigger "SolvedFast")
    }

    public enum JokerTrigger
    {
        SolvedFast,        // Resuelto antes de triggerTimeLimit segundos
        FirstAttempt,      // Sin errores previos
        Always             // Siempre activo al resolver
    }

    /// <summary>
    /// Gestiona los Jokers activos de la sesión de juego actual.
    /// Calcula el multiplicador y bonus final cuando GameManager llama a ApplyJokers().
    /// </summary>
    public class JokerManager : MonoBehaviour
    {
        public static JokerManager Instance;

        [Header("Jokers activos en esta sesión")]
        public List<JokerData> activeJokers = new List<JokerData>();

        [Header("Jokers de inicio (Fase 3 hardcodeados)")]
        public bool startWithLupaAntigua = true;

        private float levelStartTime;
        private bool hadWrongAnswer = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (startWithLupaAntigua)
            {
                activeJokers.Add(new JokerData
                {
                    jokerName = "🔍 Lupa Antigua",
                    description = "Si resuelves el caso en menos de 2 minutos, tu puntuación final se multiplica x3.",
                    trigger = JokerTrigger.SolvedFast,
                    multiplier = 3f,
                    triggerTimeLimit = 120f
                });
            }
        }

        public void OnLevelLoaded()
        {
            levelStartTime = Time.time;
            hadWrongAnswer = false;
        }

        public void OnWrongAnswer()
        {
            hadWrongAnswer = true;
        }

        /// <summary>
        /// Calcula la puntuación final aplicando todos los Jokers activos.
        /// Devuelve un JokerResult con el multiplicador total, bonus de monedas y los nombres de Jokers activados.
        /// </summary>
        public JokerResult ApplyJokers(int baseScore)
        {
            float elapsed = Time.time - levelStartTime;
            float totalMultiplier = 1f;
            int totalBonusCoins = 0;
            List<string> triggeredNames = new List<string>();

            foreach (var joker in activeJokers)
            {
                bool triggered = false;
                switch (joker.trigger)
                {
                    case JokerTrigger.SolvedFast:
                        triggered = elapsed <= joker.triggerTimeLimit;
                        break;
                    case JokerTrigger.FirstAttempt:
                        triggered = !hadWrongAnswer;
                        break;
                    case JokerTrigger.Always:
                        triggered = true;
                        break;
                }

                if (triggered)
                {
                    totalMultiplier *= joker.multiplier;
                    totalBonusCoins += joker.bonusCoins;
                    triggeredNames.Add(joker.jokerName);
                    Debug.Log($"[Joker] '{joker.jokerName}' activado! x{joker.multiplier}");
                }
            }

            int finalScore = Mathf.RoundToInt(baseScore * totalMultiplier) + totalBonusCoins;

            return new JokerResult
            {
                baseScore = baseScore,
                finalScore = finalScore,
                totalMultiplier = totalMultiplier,
                totalBonusCoins = totalBonusCoins,
                triggeredJokerNames = triggeredNames
            };
        }
    }

    [System.Serializable]
    public class JokerResult
    {
        public int baseScore;
        public int finalScore;
        public float totalMultiplier;
        public int totalBonusCoins;
        public List<string> triggeredJokerNames;
    }
}
