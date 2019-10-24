using PonyChallengeCore.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;

namespace PonyChallengeCore.Tests
{
    [TestClass]
    public class MoveTests
    {
        [TestMethod()]
        [ExpectedException(typeof(IOException))]
        public void DetermineMoveTestInvalidKey()
        {
            Program.DetermineMove(ConsoleKey.Escape);
        }

        [TestMethod()]
        public void DetermineMoveWithUserInputTestValidKeyUpArrow()
        {
            var actual = Program.DetermineMove(ConsoleKey.UpArrow);
            var expected = "north";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DetermineMoveWithUserInputTestValidKeyLeftArrow()
        {
            var actual = Program.DetermineMove(ConsoleKey.LeftArrow);
            var expected = "west";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DetermineMoveWithUserInputTestValidKeyDownArrow()
        {
            var actual = Program.DetermineMove(ConsoleKey.DownArrow);
            var expected = "south";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DetermineMoveWithUserInputTestValidKeyRightArrow()
        {
            var actual = Program.DetermineMove(ConsoleKey.RightArrow);
            var expected = "east";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DetermineMoveProgramaticallyTestValidSideNorth()
        {
            var actual = Program.DetermineMove(CellSide.North);
            var expected = "north";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DetermineMoveProgramaticallyTestValidSideWest()
        {
            var actual = Program.DetermineMove(CellSide.West);
            var expected = "west";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DetermineMoveProgramaticallyTestValidSideSouth()
        {
            var actual = Program.DetermineMove(CellSide.South);
            var expected = "south";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DetermineMoveProgramaticallyTestValidSideEast()
        {
            var actual = Program.DetermineMove(CellSide.East);
            var expected = "east";

            Assert.AreEqual(expected, actual);
        }
    }
}
