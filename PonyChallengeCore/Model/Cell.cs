using System;
using System.Collections.Generic;
using System.Text;

namespace PonyChallengeCore.Model
{
    public class Cell
    {
        public Dictionary<CellSide,CellSideState> Sides { get; }
        public CellSide? FirstEnteredSide;

        public Cell()
        {
            Sides = new Dictionary<CellSide, CellSideState>();
            Sides.Add(CellSide.North, CellSideState.Open);
            Sides.Add(CellSide.West, CellSideState.Open);
            Sides.Add(CellSide.South, CellSideState.Open);
            Sides.Add(CellSide.East, CellSideState.Open);

            FirstEnteredSide = null;
        }

    }
}
