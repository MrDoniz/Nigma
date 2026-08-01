using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Assume standard UI for prototype

namespace Nigma.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Level Data (Prototype)")]
        [TextArea(3, 5)]
        public string puzzleDescription = "El Gato jura que vio al Perro comerse la tarta desde el sofá.\nPero la Viuda dice que el sofá bloqueaba la vista hacia la ventana.\n¿Dónde estaba la tarta?";
        
        [Header("UI References")]
        public Text puzzleTextUI;
        public Button solveButton;
        public GameObject victoryPanel;

        private GridManager gridManager;
        
        private bool isResolving = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            gridManager = FindObjectOfType<GridManager>();
            
            if (puzzleTextUI != null)
            {
                puzzleTextUI.text = puzzleDescription;
            }
            
            if (solveButton != null)
            {
                solveButton.onClick.AddListener(OnSolveButtonClicked);
            }
            
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }
        }

        private void OnSolveButtonClicked()
        {
            // Enter "Resolve" mode where the next click on the grid checks victory
            isResolving = true;
            Debug.Log("Resolve mode active. Click on the correct grid cell to answer the Enigma.");
            
            if (puzzleTextUI != null)
            {
                puzzleTextUI.text = "Selecciona la casilla correcta en el tablero...";
            }
        }

        private void Update()
        {
            // If we are in resolve mode, wait for user to click on a cell
            if (isResolving && Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
                
                if (groundPlane.Raycast(ray, out float rayDistance))
                {
                    Vector3 hitPoint = ray.GetPoint(rayDistance);
                    GridManager.GridNode clickedNode = gridManager.GetNearestNode(hitPoint);
                    
                    CheckVictoryCondition(clickedNode);
                }
            }
        }

        private void CheckVictoryCondition(GridManager.GridNode clickedNode)
        {
            // For the prototype, we hardcode that the correct answer is cell (3, 3)
            int correctX = 3;
            int correctY = 3;

            if (clickedNode.x == correctX && clickedNode.y == correctY)
            {
                Debug.Log("VICTORY! You solved the enigma.");
                isResolving = false;
                
                if (victoryPanel != null)
                {
                    victoryPanel.SetActive(true);
                }
                if (puzzleTextUI != null)
                {
                    puzzleTextUI.text = "¡Correcto! Misterio resuelto.";
                }
            }
            else
            {
                Debug.Log("Wrong cell. Keep trying.");
                // Reset resolving state or penalize score
                isResolving = false;
                if (puzzleTextUI != null)
                {
                    puzzleTextUI.text = "Incorrecto. Vuelve a leer el atestado:\n\n" + puzzleDescription;
                }
            }
        }
    }
}
