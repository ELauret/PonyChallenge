//#define INTERACTIVE

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using PonyChallengeCore.Model;
using System.Threading;
using PonyChallengeCore.Infrastructure.MazeGameServices;
using PonyChallengeCore.Core.Interfaces;

namespace PonyChallengeCore
{
    public class Program
    {
        static List<ConsoleKey> ValidKeys = new List<ConsoleKey>()
                {
                    ConsoleKey.UpArrow,
                    ConsoleKey.LeftArrow,
                    ConsoleKey.DownArrow,
                    ConsoleKey.RightArrow,
                    ConsoleKey.N,
                    ConsoleKey.W,
                    ConsoleKey.S,
                    ConsoleKey.E
                };

        static readonly IMazeGameService _gameService = new HttpMazeGameService();

        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            try
            {
                string mazeId = await InitializeMaze().ConfigureAwait(false);
                Console.WriteLine(mazeId);
                await _gameService.PrintMazeAsync(mazeId).ConfigureAwait(false);

#if INTERACTIVE
                #region Logic to play the game interactively

                bool active = true;
                while (active)
                {
                    Console.WriteLine($"What is your next move (N, W, S, E)?");
                    var inputKey = Console.ReadKey().Key;

                    if (!ValidKeys.Contains(inputKey))
                    {
                        Console.WriteLine("The input is invalid.");
                        continue;
                    }

                    var statuses = await _gameService.MakeNextMoveAsync(mazeId, DetermineMove(inputKey)).ConfigureAwait(false);
                    await _gameService.PrintMazeAsync(mazeId).ConfigureAwait(false);
                    Console.WriteLine(statuses.MoveStatus);

                    if (!statuses.MazeState.Equals("active"))
                    {
                        active = false;
                        Console.WriteLine(statuses.MazeState);
                    }
                }

                #endregion

#else
                var mazeState = await _gameService.GetMazeCurrentStateAsync(mazeId).ConfigureAwait(false);
                var mazeSolver = new MazeSolver(mazeState);
                mazeSolver.SetDistanceToExit(mazeState.ExitPosition[0]);

                while (mazeState.GameState.MazeState.ToLower() == "active")
                {
                    var ponyPosition = mazeState.PonyPosition[0];

                    CellSide sideToCross = mazeSolver.FindSideToCross(ponyPosition);
                    mazeSolver.Cells[ponyPosition].Sides[sideToCross] = CellSideState.Closed;

                    var statuses = await _gameService.MakeNextMoveAsync(mazeId, DetermineMove(sideToCross)).ConfigureAwait(false);
                    if (statuses.MoveStatus.ToLower().Equals("move accepted"))
                    {
                        mazeSolver.SetFirstEnteredSideStatus(ponyPosition, sideToCross);
                    }

                    await _gameService.PrintMazeAsync(mazeId).ConfigureAwait(false);
                    Console.WriteLine(statuses.MoveStatus);

                    mazeState = await _gameService.GetMazeCurrentStateAsync(mazeId).ConfigureAwait(false);
                    if (mazeState.GameState.MazeState != "active")
                    {
                        Console.WriteLine(statuses.MazeState);
                    }

                    Thread.Sleep(250);
                }
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} Line : {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Determine the move based on user input
        /// </summary>
        /// <param name="inputKey"></param>
        /// <returns> Move as a string </returns>
        public static string DetermineMove(ConsoleKey inputKey)
        {
            var move = string.Empty;
            switch (inputKey)
            {
                case ConsoleKey k when (k == ConsoleKey.UpArrow || k == ConsoleKey.N):
                    move = "north";
                    break;
                case ConsoleKey k when (k == ConsoleKey.LeftArrow || k == ConsoleKey.W):
                    move = "west";
                    break;
                case ConsoleKey k when (k == ConsoleKey.DownArrow || k == ConsoleKey.S):
                    move = "south";
                    break;
                case ConsoleKey k when (k == ConsoleKey.RightArrow || k == ConsoleKey.E):
                    move = "east";
                    break;
                default:
                    throw new IOException("Pressed key is not in the list of valid input.");
            }

            return move;
        }

        /// <summary>
        /// Determine the move based on user input
        /// </summary>
        /// <param name="inputKey"></param>
        /// <returns> Move as a string </returns>
        public static string DetermineMove(CellSide sideToCross)
        {
            var move = string.Empty;
            switch (sideToCross)
            {
                case CellSide.North:
                    move = "north";
                    break;
                case CellSide.West:
                    move = "west";
                    break;
                case CellSide.South:
                    move = "south";
                    break;
                case CellSide.East:
                    move = "east";
                    break;
                default:
                    throw new ArgumentException("Side of the cell to cross is invalid.");
            }

            return move;
        }

        /// <summary>
        /// Looks for existing active maze game. Initialize a new one if none.
        /// </summary>
        /// <returns> Maze Id</returns>
        private static async Task<string> InitializeMaze()
        {
            const string fileName = "CurrentMaze.txt";
            string mazeId;

            if (File.Exists(fileName))
            {
                mazeId = File.ReadAllText(fileName);
                var mazeState = await _gameService.GetMazeCurrentStateAsync(mazeId).ConfigureAwait(false);
                if (mazeState.GameState.MazeState.ToLower().Equals("active"))
                {
                    return mazeId;
                }
            }

            mazeId = await InitializeMaze(15, 15, "Applejack", 10).ConfigureAwait(false);
            File.WriteAllText(fileName, mazeId);

            return mazeId;
        }

        /// <summary>
        /// Create a new maze game.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="heigth"></param>
        /// <param name="playerName"></param>
        /// <param name="difficulty"></param>
        /// <returns> Maze Id </returns>
        private static async Task<string> InitializeMaze(int width, int heigth, string playerName, int difficulty)
        {
            var maze = new MazeCreationInfo
            {
                Width = width,
                Height = heigth,
                PlayerName = playerName,
                Difficulty = difficulty
            };
            return await _gameService.CreateNewMazeGameAsync(maze).ConfigureAwait(false);
        }
    }
}