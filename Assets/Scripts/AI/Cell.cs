using UnityEngine;

namespace PandaCafe.AI
{
    //Represents a single cell in the grid used for navigation or spatial logic
    public class Cell
    {
        //Defines whether the cell is walkable or blocked
        public CellType CellType {get; private set;}
        // World-space position of the cell (usually the center)
        public Vector3 WorldPosition {get; private set;}

        public int Row {get; private set;}
        public int Column {get; private set;}

        public Vector2 Min {get; private set;}
        public Vector2 Max {get; private set;}

        public int GCost {get; set;}
        public int HCost {get; set;}
        public int FCost {get; set;}
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

        public void SetCellType(CellType CellType)
        {
            this.CellType = CellType;
        }
    }
}

