using Microsoft.VisualStudio.TestTools.UnitTesting;
using PonyChallengeCore.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PonyChallengeCore.Model.Tests
{
    [TestClass()]
    public class MazeSolverTests
    {
        MazeSolver mazeSolver;

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
                PonyPosition = new[] { 10 },
                DomokunPosition = new[] { 2 },
                ExitPosition = new[] { 18 },
                GameState = new GameState() { MazeState = "Active", MoveStatus = "Successfully created" }
            };

            mazeSolver = new MazeSolver(mazeState);
        }

        #region Tests for IdentifyAdjacentCell() method

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void IdentifyAdjacentCellTestFailCellOutsideMaze()
        {
            var cellId = mazeSolver.Width * mazeSolver.Height + 10;
            _ = mazeSolver.IdentifyAdjacentCell(cellId, CellSide.North);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void IdentifyAdjacentCellTestFailBoundarySide()
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
        public void FindAccessibleAdjacentCellsTestFailCurrentCellOutsideMaze()
        {
            var cellId = mazeSolver.Width * mazeSolver.Height + 10;
            _ = mazeSolver.FindAccessibleAdjacentCells(cellId, null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void FindAccessibleAdjacentCellsTestFailComingCellOutsideMaze()
        {
            var cellId = (mazeSolver.Width * mazeSolver.Height) / 2;
            var comingCellId = mazeSolver.Width * mazeSolver.Height + 10;
            _ = mazeSolver.FindAccessibleAdjacentCells(cellId, comingCellId);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void FindAccessibleAdjacentCellsTestFailComingCellSameAsCurrent()
        {
            var cellId = (mazeSolver.Width * mazeSolver.Height) / 2;
            var comingCellId = cellId;
            _ = mazeSolver.FindAccessibleAdjacentCells(cellId, comingCellId);
        }

        [TestMethod()]
        public void FindAccessibleAdjacentCellsTestPassNoComingCell()
        {
            var cellId = 12;
            var result = mazeSolver.FindAccessibleAdjacentCells(cellId, null);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(i => i == 7));
            Assert.IsTrue(result.Any(i => i == 13));
        }

        [TestMethod()]
        public void FindAccessibleAdjacentCellsTestPassComingCell()
        {
            var cellId = 12;
            var comingCellId = 13;
            var result = mazeSolver.FindAccessibleAdjacentCells(cellId, comingCellId);

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Any(i => i == 7));
            Assert.IsFalse(result.Any(i => i == comingCellId));
        }

        [TestMethod()]
        public void FindAccessibleAdjacentCellsTestPassNoAccessibleAdjacentCell()
        {
            var cellId = 24;
            var comingCellId = 19;
            var result = mazeSolver.FindAccessibleAdjacentCells(cellId, comingCellId);

            Assert.AreEqual(0, result.Count);
        }

        #endregion
    }
}