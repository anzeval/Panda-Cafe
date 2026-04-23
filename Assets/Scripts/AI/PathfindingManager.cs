using System.Collections.Generic;
using UnityEngine;

namespace PandaCafe.AI
{
    public class PathfindingManager
    {
        private GridManager gridManager;

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

        public List<Cell> FindPathfFromVector3(Vector3 start, Vector3 target)
        {
            if(!gridManager.TryGetGridCoordinates(start, out Vector2Int startCellCoordinates)) return null;
            if(!gridManager.TryGetGridCoordinates(target, out Vector2Int targetCellCoordinates)) return null;
            
            if(!gridManager.TryGetCell(startCellCoordinates.y, startCellCoordinates.x, out Cell startCell)) return null;
            if(!gridManager.TryGetCell(targetCellCoordinates.y, targetCellCoordinates.x, out Cell targetCell)) return null;

            return FindPath(startCell, targetCell);
        }

        public List<Cell> FindPath(Cell start, Cell goal)
        {
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
                Cell current = openList[0];

                foreach(Cell cell in openList)
                {
                    if(cell.FCost < current.FCost || (cell.FCost == current.FCost && cell.HCost < current.HCost))
                    {
                        current = cell;
                    }
                }

                if(current == goal) return ReconstructPath(goal);;

                openList.Remove(current);
                closedList.Add(current);

                List<Cell> neighbors = GetNeighbors(current);

                foreach (Cell neighbor in neighbors)
                {
                    if(closedList.Contains(neighbor) || neighbor.CellType == CellType.Unwalkable)
                        continue;

                    if (IsDiagonalMove(current, neighbor) && IsDiagonalBlocked(current, neighbor))
                        continue;

                    int gCost = current.GCost + Distance(current, neighbor);

                    bool isInOpen = openList.Contains(neighbor);

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
        private int Heuristic(Vector2 start, Vector2 finish)
        {
            float xSide = Mathf.Pow((start.x - finish.x), 2f);
            float ySide = Mathf.Pow((start.y - finish.y), 2f);
            int result = (int)(Mathf.Sqrt((xSide + ySide) * 10));
            return result;
        }

       private int Distance(Cell current, Cell neighbor)
        {
            if(Mathf.Abs(current.Row - neighbor.Row) == 1 && Mathf.Abs(current.Column - neighbor.Column) == 1)
                return 14;

            return 10; 
        } 

        private bool IsDiagonalMove(Cell current, Cell neighbor)
        {
            return Mathf.Abs(current.Row - neighbor.Row) == 1 && Mathf.Abs(current.Column - neighbor.Column) == 1;
        }

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
