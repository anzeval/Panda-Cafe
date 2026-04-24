using System.Collections.Generic;
using UnityEngine;

namespace PandaCafe.AI
{
    // Handles A* pathfinding on grid and returns path between two points.
    // Converts world positions to cells, finds valid path, handles blocked targets.
    public class PathfindingManager
    {
        private GridManager gridManager;

        // Relative offsets to get all 8 neighboring cells (including diagonals)
        private List<Vector2Int> neighborsCoordinates = new List<Vector2Int>
        {
                new Vector2Int(1,-1),
                new Vector2Int(1,0),
                new Vector2Int(1,1),
                new Vector2Int(0,-1),
                new Vector2Int(0,1),
                new Vector2Int(-1,-1),
                new Vector2Int(-1,0),
                new Vector2Int(-1,1)
        };

        public void Init(GridManager gridManager)
        {
            this.gridManager = gridManager;
        }

        // Converts world positions to cells and starts pathfinding
        public List<Cell> FindPathfFromVector3(Vector3 start, Vector3 target)
        {
            if(!gridManager.TryGetGridCoordinates(start, out Vector2Int startCellCoordinates)) return null;
            if(!gridManager.TryGetGridCoordinates(target, out Vector2Int targetCellCoordinates)) return null;
            
            if(!gridManager.TryGetCell(startCellCoordinates.y, startCellCoordinates.x, out Cell startCell)) return null;
            if(!gridManager.TryGetCell(targetCellCoordinates.y, targetCellCoordinates.x, out Cell targetCell)) return null;

            if (startCell.CellType == CellType.Unwalkable)
                return null;

            // If target is blocked, try nearest walkable cell
            if (targetCell.CellType == CellType.Unwalkable)
            {
                if (!TryGetNearestWalkableCell(targetCell, out Cell walkableTargetCell))
                {
                    return null;
                }

                targetCell = walkableTargetCell;
            }

            return FindPath(startCell, targetCell);
        }

        // A* pathfinding: finds shortest path between two cells
        public List<Cell> FindPath(Cell start, Cell goal)
        {
            // Reset all cells
            foreach (Cell cell in gridManager.cells)
            {
                cell.GCost = int.MaxValue;
                cell.HCost = 0;
                cell.FCost = 0;
                cell.Parent = null;
            }

            List<Cell> openList = new List<Cell>();
            HashSet<Cell> closedList = new HashSet<Cell>();

            openList.Add(start);

            start.GCost = 0;
            start.HCost = Heuristic(new Vector2(start.Column, start.Row), new Vector2(goal.Column, goal.Row));
            start.FCost = start.GCost + start.HCost;
            start.Parent = null;

            while (openList.Count > 0)
            {
                // Pick node with lowest F cost
                Cell current = openList[0];

                foreach(Cell cell in openList)
                {
                    if(cell.FCost < current.FCost || (cell.FCost == current.FCost && cell.HCost < current.HCost))
                    {
                        current = cell;
                    }
                }

                // Path found
                if(current == goal) return ReconstructPath(goal);;

                openList.Remove(current);
                closedList.Add(current);

                List<Cell> neighbors = GetNeighbors(current);

                foreach (Cell neighbor in neighbors)
                {
                    // Skip invalid nodes
                    if(closedList.Contains(neighbor) || neighbor.CellType == CellType.Unwalkable)
                        continue;

                    // Prevent diagonal cutting through walls
                    if (IsDiagonalMove(current, neighbor) && IsDiagonalBlocked(current, neighbor))
                        continue;

                    int gCost = current.GCost + Distance(current, neighbor);

                    bool isInOpen = openList.Contains(neighbor);

                    // Update better path
                    if (!isInOpen || gCost < neighbor.GCost)
                    {
                        neighbor.GCost = gCost;
                        neighbor.HCost = Heuristic(new Vector2(neighbor.Column, neighbor.Row), new Vector2(goal.Column, goal.Row));
                        neighbor.FCost = neighbor.GCost + neighbor.HCost;
                        neighbor.Parent = current;

                        if (!isInOpen)
                            openList.Add(neighbor);
                    }
                }
            }
            return null;
        }

        // Finds closest walkable cell around blocked target
        private bool TryGetNearestWalkableCell(Cell targetCell, out Cell walkableCell)
        {
            walkableCell = null;
            int maxRadius = Mathf.Max(gridManager.cells.GetLength(0), gridManager.cells.GetLength(1));

            // Expanding ring search
            for (int radius = 1; radius < maxRadius; radius++)
            {
                for (int row = targetCell.Row - radius; row <= targetCell.Row + radius; row++)
                {
                    for (int column = targetCell.Column - radius; column <= targetCell.Column + radius; column++)
                    {
                        // Only check ring border
                        bool isOnRingBorder = row == targetCell.Row - radius || row == targetCell.Row + radius || column == targetCell.Column - radius || column == targetCell.Column + radius;

                        if (!isOnRingBorder)
                            continue;

                        if (!gridManager.TryGetCell(row, column, out Cell cell))
                            continue;

                        if (cell.CellType == CellType.Walkable)
                        {
                            walkableCell = cell;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // Reconstructs path by following parent links
        public List<Cell> ReconstructPath(Cell goal)
        {   
            var path = new List<Cell>();
            Cell current = goal;

            while (current != null)
            {
                path.Insert(0, current);
                current = current.Parent;
            }

            return path;
        }

        // Returns all valid neighboring cells
        private List<Cell> GetNeighbors(Cell current)
        {
            List<Cell> neighbors = new List<Cell>();

            foreach (Vector2Int neighborCoordinate in neighborsCoordinates)
            {
                int row = neighborCoordinate.y + current.Row;
                int column = neighborCoordinate.x + current.Column;

                if(gridManager.TryGetCell(row, column, out Cell cell))
                {
                    neighbors.Add(cell);
                }
            }

            return neighbors;
        }

        // Heuristic distance (Euclidean * 10)
        private int Heuristic(Vector2 start, Vector2 finish)
        {
            float xSide = Mathf.Pow((start.x - finish.x), 2f);
            float ySide = Mathf.Pow((start.y - finish.y), 2f);
            int result = (int)(Mathf.Sqrt((xSide + ySide) * 10));
            return result;
        }

        // Movement cost between cells (straight vs diagonal)
       private int Distance(Cell current, Cell neighbor)
        {
            if(Mathf.Abs(current.Row - neighbor.Row) == 1 && Mathf.Abs(current.Column - neighbor.Column) == 1)
                return 14;// diagonal

            return 10; // straight
        } 

        // Checks if move is diagonal
        private bool IsDiagonalMove(Cell current, Cell neighbor)
        {
            return Mathf.Abs(current.Row - neighbor.Row) == 1 && Mathf.Abs(current.Column - neighbor.Column) == 1;
        }

        // Prevents moving diagonally through blocked corners
        private bool IsDiagonalBlocked(Cell current, Cell neighbor)
        {
            int horizontalColumn = neighbor.Column;
            int horizontalRow = current.Row;

            int verticalColumn = current.Column;
            int verticalRow = neighbor.Row;

            bool hasHorizontal = gridManager.TryGetCell(horizontalRow, horizontalColumn, out Cell horizontalCell);
            bool hasVertical = gridManager.TryGetCell(verticalRow, verticalColumn, out Cell verticalCell);

            if (!hasHorizontal || !hasVertical)
                return true;

            return horizontalCell.CellType == CellType.Unwalkable || verticalCell.CellType == CellType.Unwalkable;
        } 
    }
}
