using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PonyChallengeCore.Model
{
    public class MazeSolver
    {        
        public Cell[] Cells { get; set; }
        public int Width { get; }
        public int Height { get; }

        public MazeSolver(MazeState mazeState)
        {
            if (mazeState == null) throw new ArgumentNullException("mazeState");
            if (mazeState.Width * mazeState.Height == 0) throw new ArgumentException("Width and height of the maze cannot be zero!");

            Width = mazeState.Width;
            Height = mazeState.Height;

            Cells = new Cell[Width * Height];

            for (int i = 0; i < Width * Height; i++)
            {
                Cells[i] = new Cell();

                if (mazeState.Data[i].Any(w => w == "north")) Cells[i].Sides[CellSide.North] = CellSideState.Sealed;
                if (mazeState.Data[i].Any(w => w == "west")) Cells[i].Sides[CellSide.West] = CellSideState.Sealed;

                if (i < Width * Height - 1)
                {
                    if (mazeState.Data[i + 1].Any(w => w == "west")) Cells[i].Sides[CellSide.East] = CellSideState.Sealed;
                }
                else
                {
                    Cells[i].Sides[CellSide.East] = CellSideState.Sealed;
                }

                if (i < Width * (Height - 1))
                {
                    if (mazeState.Data[i + Width].Any(w => w == "north")) Cells[i].Sides[CellSide.South] = CellSideState.Sealed;
                }
                else
                {
                    Cells[i].Sides[CellSide.South] = CellSideState.Sealed;
                }
            }
        }

        /// <summary>
        /// Find for the current cell which side of it to go through to the next cell
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns> Returns the side of the cell to go through </returns>
        public CellSide FindSideToCross(Cell currentCell)
        {
            var openSidesCount = currentCell.Sides.Count(s => s.Value == CellSideState.Open);

            CellSide sideToCross;
            if (openSidesCount > 1)
            {
                sideToCross = currentCell.Sides.First(
                                    s => s.Value == CellSideState.Open
                                    && s.Key != currentCell.FirstEnteredSide).Key;
            }
            else if (openSidesCount == 1)
            {
                sideToCross = currentCell.Sides.Single(s => s.Value == CellSideState.Open).Key;
            }
            else
            {
                if (currentCell.FirstEnteredSide != null)
                {
                    sideToCross = (CellSide)currentCell.FirstEnteredSide;
                }
                else
                {
                    throw new Exception("Houston... We have a problem.");
                }
            }

            return sideToCross;
        }

        /// <summary>
        /// When entering for the first time in a cell, mark the entry side as such to make sure it will be the last option for the next moves
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="sideToCross"></param>
        public void SetFirstEnteredSideStatus(int currentPosition, CellSide sideToCross)
        {
            switch (sideToCross)
            {
                case CellSide.North:
                    if (Cells[currentPosition - Width].FirstEnteredSide == null)
                    {
                        Cells[currentPosition - Width].FirstEnteredSide = CellSide.South;
                    }
                    break;
                case CellSide.West:
                    if (Cells[currentPosition - 1].FirstEnteredSide == null) Cells[currentPosition - 1].FirstEnteredSide = CellSide.East;
                    break;
                case CellSide.South:
                    if (Cells[currentPosition + Width].FirstEnteredSide == null)
                    {
                        Cells[currentPosition + Width].FirstEnteredSide = CellSide.North;
                    }
                    break;
                case CellSide.East:
                    if (Cells[currentPosition + 1].FirstEnteredSide == null) Cells[currentPosition + 1].FirstEnteredSide = CellSide.West;
                    break;
                default:
                    break;
            }
        }
    }
}
