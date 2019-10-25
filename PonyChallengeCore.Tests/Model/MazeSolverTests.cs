using Microsoft.VisualStudio.TestTools.UnitTesting;
using PonyChallengeCore.Model;
using System;
using System.Collections.Generic;
using System.Text;

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

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void IdentifyAdjacentCellTestFailedBoundarySide()
        {
            var cellId = 3;
            var actual = mazeSolver.IdentifyAdjacentCell(cellId, CellSide.North);
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
    }
}