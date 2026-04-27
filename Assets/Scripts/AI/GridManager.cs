using UnityEngine;

namespace PandaCafe.AI
{
    // Manages a 2D grid overlay used for navigation or spatial queries
    // Responsible for generating cells and determining their walkability
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private float cellSize = 0.2f;
        [SerializeField] private LayerMask layerMask; //  used to detect obstacles

        public Cell[,] cells {get; private set;}

        private int gridWidth;
        private int gridHeight;

        private Vector2 size;
        private Vector2 gridMin;
        private Vector2 gridMax;

        // Physics filter used to detect only relevant colliders (excluding triggers)
        private ContactFilter2D filter;
        // Reusable buffer for physics queries to avoid allocations
        private Collider2D[] results = new Collider2D[15];  

        public void Init(SpriteRenderer background)
        {
            CalculateGrid(background);
            InitializeGrid();
        }

        // Sets up the physics filter
        void Awake()
        {
            filter = new ContactFilter2D();
            filter.SetLayerMask(layerMask);
            filter.useTriggers = false; 
        }

        // Calculates grid dimensions and bounds from the background
        private void CalculateGrid(SpriteRenderer background)
        {
            size = background.bounds.size;
            gridMin = background.bounds.min;
            gridMax = background.bounds.max;

            gridWidth = (int)(size.x / cellSize);
            gridHeight = (int)(size.y / cellSize);

            cells = new Cell[gridHeight, gridWidth];
        }

        // Creates and initializes all cells in the grid
        private void InitializeGrid()
        {
            for (int row = 0; row < gridHeight; row++)
            {
                for (int column = 0; column < gridWidth; column++)
                {
                    Vector3 position = GetWorldPosition(row, column);

                    Vector2 min = new Vector2(position.x - (cellSize / 2), position.y - (cellSize / 2));
                    Vector2 max = new Vector2(position.x + (cellSize / 2), position.y + (cellSize / 2));
        
                    Cell cell = new Cell(DefineCellType(min, max), position, row, column, min, max);
                    cells[row,column] = cell;
                }
            }
        }

        // Attempts to retrieve a cell by its grid coordinates
        public bool TryGetCell(int row, int column, out Cell cell)
        {
            if(cells == null || cells.Length <= 0 || row < 0 || row >= gridHeight || column < 0 || column >= gridWidth)
            {
                cell = default;
                return false;
            }

            cell = cells[row, column];
            return true;
        }

        // Converts grid coordinates to a world-space position
        private Vector3 GetWorldPosition(int row, int column)
        {
            float x = gridMin.x + (column * cellSize) + (cellSize / 2f);
            float y = gridMax.y - (row * cellSize) - (cellSize / 2f);

            return new Vector3(x, y, 0f);
        }

        // Converts a world-space position to grid coordinates
        public bool TryGetGridCoordinates(Vector3 worldPosition, out Vector2Int cell)
        {
            if(worldPosition.x >= gridMin.x && worldPosition.x < gridMax.x && worldPosition.y >= gridMin.y && worldPosition.y < gridMax.y)
            {
                int row = Mathf.FloorToInt((gridMax.y - worldPosition.y) / cellSize);
                int column = Mathf.FloorToInt((worldPosition.x - gridMin.x) / cellSize);

                cell = new Vector2Int(column, row);

                return true;
            }
            
            cell = default;
            return false;
        }

        // Determines whether a cell is walkable by checking for obstacles in its area
        private CellType DefineCellType(Vector2 min, Vector2 max)
        {
            int count = Physics2D.OverlapArea(min, max, filter, results);

            return (count > 0) ? CellType.Unwalkable : CellType.Walkable;
        }
    }   
}