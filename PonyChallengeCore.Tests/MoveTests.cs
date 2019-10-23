using PonyChallengeCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace PonyChallengeCore.Tests
{
    [TestClass]
    public class MoveTests
    {
        [TestMethod()]
        [ExpectedException(typeof(IOException))]
        public void DetermineMoveTestInvalidKey()
        {
            Program.DetermineMove(System.ConsoleKey.Escape);
        }

        [TestMethod()]
        public void DetermineMoveTestValidKeyUpArrow()
        {
            var move = Program.DetermineMove(System.ConsoleKey.UpArrow);
            var expected = "north";

            Assert.AreEqual(expected, move);
        }

        [TestMethod()]
        public void DetermineMoveTestValidKeyLeftArrow()
        {
            var move = Program.DetermineMove(System.ConsoleKey.LeftArrow);
            var expected = "west";

            Assert.AreEqual(expected, move);
        }

        [TestMethod()]
        public void DetermineMoveTestValidKeyDownArrow()
        {
            var move = Program.DetermineMove(System.ConsoleKey.DownArrow);
            var expected = "south";

            Assert.AreEqual(expected, move);
        }

        [TestMethod()]
        public void DetermineMoveTestValidKeyRightArrow()
        {
            var move = Program.DetermineMove(System.ConsoleKey.RightArrow);
            var expected = "east";

            Assert.AreEqual(expected, move);
        }
    }
}
