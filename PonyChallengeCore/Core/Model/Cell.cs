using System;
using System.Collections.Generic;
using System.Text;

namespace PonyChallengeCore.Model
{
    public class Cell
    {
        public Dictionary<CellSide,CellSideState> Sides { get; }
        public int? PreviousCellIdFromExit { get; set; }
        public int? DistanceToExit { get; set; }
        public CellSide? FirstEnteredSide { get; set; }

        public Cell()
        {
            Sides = new Dictionary<CellSide, CellSideState>();
            Sides.Add(CellSide.North, CellSideState.Open);
            Sides.Add(CellSide.West, CellSideState.Open);
            Sides.Add(CellSide.South, CellSideState.Open);
            Sides.Add(CellSide.East, CellSideState.Open);

            PreviousCellIdFromExit = null;
            DistanceToExit = null;
            FirstEnteredSide = null;
        }

    }
}
