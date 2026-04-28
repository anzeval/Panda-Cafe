using UnityEngine;

namespace PandaCafe.AI
{
    // Grid cell for navigation
    public class Cell
    {
        // Walkable or blocked
        public CellType CellType {get; private set;}

        // Center position in world space
        public Vector3 WorldPosition {get; private set;}

        // Grid coordinates
        public int Row {get; private set;}
        public int Column {get; private set;}

        // Cell bounds
        public Vector2 Min {get; private set;}
        public Vector2 Max {get; private set;}

        // A* costs
        public int GCost {get; set;}
        public int HCost {get; set;}
        public int FCost {get; set;}

        // Parent for path reconstruction
        public Cell Parent {get; set;}

        public Cell(CellType CellType, Vector3 worldPosition, int Row, int Column, Vector2 Min, Vector2 Max)
        {
            this.CellType = CellType;
            this.WorldPosition = worldPosition;

            this.Row = Row;
            this.Column = Column;

            this.Min = Min;
            this.Max = Max;
        }

        // Update cell type
        public void SetCellType(CellType CellType)
        {
            this.CellType = CellType;
        }
    }
}