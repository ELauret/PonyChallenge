using Microsoft.VisualStudio.TestTools.UnitTesting;
using PonyChallengeCore.Infrastructure.MazeGameServices;
using System;
using System.Linq;

namespace PonyChallengeCore.Model.Tests
{
    [TestClass()]
    public class MazeSolverTests
    {
        MazeSolver mazeSolver;
        const int ponyPosition = 10;
        const int domokunPosition = 2;
        const int exitPosition = 18;

        [TestInitialize]
        public void InitializeMazeSolverTests()
        {
            var width = 5;
            var heigth = 5;
            var mazeData = new string[][]
                {
                    new[] {"north", "west"}, new[]{ "north", "west" }, new[]{ "north" }, new[]{ "north"}, new[]{ "north"},
                    new[]{"west"}, new[]{"north"}, new[]{"north", "west"}, new string[0], new[]{"west"},
                    new[]{"north", "west"}, new string[0], new[]{"west"}, new[]{"north"}, new[]{"west"},
                    new[]{"west"}, new[]{"north", "west"}, new[]{ "north"}, new[]{ "west"}, new[]{ "west"},
                    new[]{"west"}, new string[0], new[]{"west"}, new string[0], new[]{"west"}
                };

            var mazeState = new MazeState()
            {
                Id = "TestMaze1234",
                Difficulty = 1,
                PlayerName = "moi",
                Dimensions = new[] { width, heigth },
                Data = mazeData,
                PonyPosition = new[] { ponyPosition },
                DomokunPosition = new[] { domokunPosition },
                ExitPosition = new[] { exitPosition },
                GameState = new GameState() { MazeState = "Active", MoveStatus = "Successfully created" }
            };

            mazeSolver = new MazeSolver(mazeState);
        }

        #region Tests for IdentifyAdjacentCell() method

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void IdentifyAdjacentCellTestCellOutsideMaze()
        {
            var cellId = mazeSolver.Width * mazeSolver.Height + 10;
            _ = mazeSolver.IdentifyAdjacentCell(cellId, CellSide.North);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void IdentifyAdjacentCellTestBoundarySide()
        {
            var cellId = 3;
            _ = mazeSolver.IdentifyAdjacentCell(cellId, CellSide.North);
        }

        [TestMethod()]
        public void IdentifyAdjacentCellTestOnNorthSide()
        {
            var cellId = 12;
            var actual = mazeSolver.IdentifyAdjacentCell(cellId, CellSide.North);
            var expected = cellId - mazeSolver.Width;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IdentifyAdjacentCellTestOnWestSide()
        {
            var cellId = 12;
            var actual = mazeSolver.IdentifyAdjacentCell(cellId, CellSide.West);
            var expected = cellId - 1;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IdentifyAdjacentCellTestOnSouthSide()
        {
            var cellId = 12;
            var actual = mazeSolver.IdentifyAdjacentCell(cellId, CellSide.South);
            var expected = cellId + mazeSolver.Width;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IdentifyAdjacentCellTestOnEastSide()
        {
            var cellId = 12;
            var actual = mazeSolver.IdentifyAdjacentCell(cellId, CellSide.East);
            var expected = cellId + 1;

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Tests for FindAccessibleAdjacentCells() method

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void FindAccessibleAdjacentCellsTestCurrentCellOutsideMaze()
        {
            var cellId = mazeSolver.Width * mazeSolver.Height + 10;
            _ = mazeSolver.FindAccessibleAdjacentCells(cellId, null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void FindAccessibleAdjacentCellsTestComingCellOutsideMaze()
        {
            var cellId = (mazeSolver.Width * mazeSolver.Height) / 2;
            var comingCellId = mazeSolver.Width * mazeSolver.Height + 10;
            _ = mazeSolver.FindAccessibleAdjacentCells(cellId, comingCellId);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void FindAccessibleAdjacentCellsTestComingCellSameAsCurrent()
        {
            var cellId = (mazeSolver.Width * mazeSolver.Height) / 2;
            var comingCellId = cellId;
            _ = mazeSolver.FindAccessibleAdjacentCells(cellId, comingCellId);
        }

        [TestMethod()]
        public void FindAccessibleAdjacentCellsTestNoComingCell()
        {
            var cellId = 12;
            var result = mazeSolver.FindAccessibleAdjacentCells(cellId, null);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(i => i == 7));
            Assert.IsTrue(result.Any(i => i == 13));
        }

        [TestMethod()]
        public void FindAccessibleAdjacentCellsTestComingCell()
        {
            var cellId = 12;
            var comingCellId = 13;
            var result = mazeSolver.FindAccessibleAdjacentCells(cellId, comingCellId);

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Any(i => i == 7));
            Assert.IsFalse(result.Any(i => i == comingCellId));
        }

        [TestMethod()]
        public void FindAccessibleAdjacentCellsTestNoAccessibleAdjacentCell()
        {
            var cellId = 24;
            var comingCellId = 19;
            var result = mazeSolver.FindAccessibleAdjacentCells(cellId, comingCellId);

            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region Tests for SetDistanceToExit() method

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetDistanceToExitTestCurrentCellOutsideMaze()
        {
            var cellId = mazeSolver.Width * mazeSolver.Height + 10;
            mazeSolver.SetDistanceToExit(cellId);
        }

        [TestMethod]
        public void SetDistanceToExitTestSimplyConnectedtMaze()
        {
            mazeSolver.SetDistanceToExit(exitPosition);

            var maxDistance = mazeSolver.Cells.Max(c => c.DistanceToExit);

            Assert.AreEqual(9, mazeSolver.Cells[11].DistanceToExit);
            Assert.AreEqual(12, maxDistance);
        }

        [TestMethod]
        public void SetDistanceToExitTestMultiplyConnectedtMaze()
        {
            mazeSolver.Cells[11].Sides[CellSide.East] = CellSideState.Open;
            mazeSolver.Cells[12].Sides[CellSide.West] = CellSideState.Open;

            mazeSolver.SetDistanceToExit(exitPosition);

            var maxDistance = mazeSolver.Cells.Max(c => c.DistanceToExit);

            Assert.AreEqual(3, mazeSolver.Cells[11].DistanceToExit);
            Assert.AreEqual(10, maxDistance);
        }

        #endregion
    }
}