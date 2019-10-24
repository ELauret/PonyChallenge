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

            InitalizeMazeCells(mazeState);
            SetCellDistanceToExit(mazeState.ExitPosition[0], null);
        }

        private void InitalizeMazeCells(MazeState mazeState)
        {
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

        public void SetCellDistanceToExit(int cellId, int? parentCellId)
        {
            if (parentCellId == null) Cells[cellId].DistanceToExit = 0;
            else Cells[cellId].DistanceToExit = Cells[(int)parentCellId].DistanceToExit + 1;

            var validNeighboursIds = FindValidNeighbourCells(cellId, parentCellId);
            foreach (var neighbourId in validNeighboursIds)
            {
                SetCellDistanceToExit(neighbourId, cellId);
            }
        }

        public List<int> FindValidNeighbourCells(int cellId, int? parentCellId)
        {
            var neighbourCellIds = new List<int>();

            var validOpenSides = Cells[cellId].Sides.Where(s => s.Value == CellSideState.Open).Select(s => s.Key);
            foreach (var side in validOpenSides)
            {
                var neighbourCellId = IdentifyNeighbourCell(cellId, side);
                if (neighbourCellId != parentCellId) neighbourCellIds.Add(neighbourCellId);
            }

            return neighbourCellIds;
        }

        public int IdentifyNeighbourCell(int cellId, CellSide side)
        {
            switch (side)
            {
                case CellSide.North:
                    return cellId - Width;
                case CellSide.West:
                    return cellId - 1;
                case CellSide.South:
                    return cellId + Width;
                case CellSide.East:
                    return cellId + 1;
                default:
                    throw new ArgumentException("The side identifier passed is not valid.");
            }
        }

        /// <summary>
        /// Find for the current cell which side of it to go through to the next cell
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns> Returns the side of the cell to go through </returns>
        public CellSide FindSideToCross(int currentCellId)
        {
            var currentCell = Cells[currentCellId];
            var openSidesCount = currentCell.Sides.Count(s => s.Value == CellSideState.Open);

            CellSide sideToCross;
            if (openSidesCount > 1)
            {
                sideToCross = currentCell.Sides.First(
                                    s => s.Value == CellSideState.Open
                                    && s.Key != currentCell.FirstEnteredSide).Key;

                var validSides = currentCell.Sides.Where(
                                    s => s.Value == CellSideState.Open
                                    && s.Key != currentCell.FirstEnteredSide).Select(s => s.Key);

                var distanceToExit = int.MaxValue;
                foreach (var side in validSides)
                {
                    var neighbourCellId = IdentifyNeighbourCell(currentCellId, side);
                    if (Cells[neighbourCellId].DistanceToExit < distanceToExit)
                    {
                        sideToCross = side;
                        distanceToExit = Cells[neighbourCellId].DistanceToExit;
                    }
                }
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
