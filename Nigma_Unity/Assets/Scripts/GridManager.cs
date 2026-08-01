using System.Collections.Generic;
using UnityEngine;

namespace Nigma.Core
{
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        public int width = 4;
        public int height = 4;
        public float cellSize = 2f;

        [Header("Prefabs")]
        public GameObject structuralWallPrefab;
        
        private List<GameObject> spawnedWalls = new List<GameObject>();

        // Represents a tile on the board
        public class GridNode
        {
            public int x;
            public int y;
            public Vector3 worldPosition;
            public DraggableObject occupant;
            public bool isStructuralWall;
        }

        private GridNode[,] grid;

        private void Awake()
        {
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            grid = new GridNode[width, height];
            Vector3 originPosition = transform.position;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = new GridNode
                    {
                        x = x,
                        y = y,
                        worldPosition = originPosition + new Vector3(x * cellSize, 0, y * cellSize),
                        occupant = null,
                        isStructuralWall = false // Set this via level generation later
                    };
                }
            }
        }

        public void ClearGrid()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] != null)
                    {
                        grid[x, y].occupant = null;
                        grid[x, y].isStructuralWall = false;
                    }
                }
            }

            foreach (var wall in spawnedWalls)
            {
                if (wall != null) Destroy(wall);
            }
            spawnedWalls.Clear();
        }

        public void SpawnStructuralWall(int x, int y)
        {
            if (IsValidNode(x, y))
            {
                grid[x, y].isStructuralWall = true;
                if (structuralWallPrefab != null)
                {
                    Vector3 pos = GetWorldPosition(x, y);
                    GameObject wall = Instantiate(structuralWallPrefab, pos, Quaternion.identity, this.transform);
                    spawnedWalls.Add(wall);
                }
            }
        }

        /// <summary>
        /// Returns the center world position for a given (x,y) grid coordinate.
        /// </summary>
        public Vector3 GetWorldPosition(int x, int y)
        {
            if (IsValidNode(x, y))
            {
                return grid[x, y].worldPosition;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Returns the nearest valid GridNode to a given world position.
        /// Useful for snapping dropped items.
        /// </summary>
        public GridNode GetNearestNode(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt((worldPos.x - transform.position.x) / cellSize);
            int y = Mathf.RoundToInt((worldPos.z - transform.position.z) / cellSize);

            x = Mathf.Clamp(x, 0, width - 1);
            y = Mathf.Clamp(y, 0, height - 1);

            return grid[x, y];
        }

        public bool IsValidNode(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public bool IsCellOccupied(GridNode node)
        {
            return node.occupant != null || node.isStructuralWall;
        }

        // Draw grid gizmos in editor for visualization
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Vector3 originPosition = transform.position;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 center = originPosition + new Vector3(x * cellSize, 0, y * cellSize);
                    Gizmos.DrawWireCube(center, new Vector3(cellSize, 0.1f, cellSize));
                }
            }
        }
    }
}
