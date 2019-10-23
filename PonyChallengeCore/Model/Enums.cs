using System;
using System.Collections.Generic;
using System.Text;

namespace PonyChallengeCore.Model
{
    public enum CellSide
    {
        North = 0,
        West = 1,
        South = 2,
        East = 3
    }

    public enum CellSideState
    {
        Open = 1,
        Closed = 2,
        Sealed = 3
    }
}
