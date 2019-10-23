//#define INTERACTIVE

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using PonyChallengeCore.Model;
using System.Linq;
using System.Threading;

namespace PonyChallengeCore
{
    public class Program
    {
        static HttpClient client = new HttpClient();
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

        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri($"https://ponychallenge.trustpilot.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                string mazeId = await InitializeMaze();
                Console.WriteLine(mazeId);
                await PrintMaze(mazeId);

#if INTERACTIVE

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

                    var statuses = await MakeNextMove(mazeId, DetermineMove(inputKey));
                    await PrintMaze(mazeId);
                    Console.WriteLine(statuses.MoveStatus);

                    if (!statuses.MazeState.Equals("active"))
                    {
                        active = false;
                        Console.WriteLine(statuses.MazeState);
                    }
                }

#else
                var mazeState = await GetMazeCurrentState(mazeId);
                var maze = InitializeMazeSolver(mazeState);
                var width = mazeState.Dimensions[0];

                while (mazeState.GameState.MazeState.ToLower() == "active")
                {
                    var ponyPosition = mazeState.PonyPosition[0];
                    var currentCell = maze[ponyPosition];
                    var openSidesCount = currentCell.Sides.Count(s => s.Value == CellSideState.Open);

                    CellSide sideToCross;
                    if (openSidesCount > 1)
                    {
                        sideToCross = currentCell.Sides.First(
                                            s => s.Value == CellSideState.Open
                                            && s.Key != currentCell.FirstEnteredSide).Key;
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
                    currentCell.Sides[sideToCross] = CellSideState.Closed;

                    var statuses = await MakeNextMove(mazeId, DetermineMove(sideToCross));

                    if (statuses.MoveStatus.ToLower().Equals("move accepted"))
                    {
                        switch (sideToCross)
                        {
                            case CellSide.North:
                                if (maze[ponyPosition - width].FirstEnteredSide == null) maze[ponyPosition - width].FirstEnteredSide = CellSide.South;
                                break;
                            case CellSide.West:
                                if (maze[ponyPosition - 1].FirstEnteredSide == null) maze[ponyPosition - 1].FirstEnteredSide = CellSide.East;
                                break;
                            case CellSide.South:
                                if (maze[ponyPosition + width].FirstEnteredSide == null) maze[ponyPosition + width].FirstEnteredSide = CellSide.North;
                                break;
                            case CellSide.East:
                                if (maze[ponyPosition + 1].FirstEnteredSide == null) maze[ponyPosition + 1].FirstEnteredSide = CellSide.West;
                                break;
                            default:
                                break;
                        }
                    }

                    await PrintMaze(mazeId);
                    Console.WriteLine(statuses.MoveStatus);

                    mazeState = await GetMazeCurrentState(mazeId);

                    if (mazeState.GameState.MazeState != "active")
                    {
                        Console.WriteLine(statuses.MazeState);
                    }

                    Thread.Sleep(500);
                }
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} Line : {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Instanciate and initialize an array of cells that represent the maze
        /// </summary>
        /// <param name="mazeState"></param>
        /// <returns></returns>
        private static Cell[] InitializeMazeSolver(MazeState mazeState)
        {
            var width = mazeState.Dimensions[0];
            var height = mazeState.Dimensions[1];
            var maze = new Cell[width * height];

            for (int i = 0; i < width * height; i++)
            {
                maze[i] = new Cell();

                if (mazeState.Data[i].Any(w => w == "north")) maze[i].Sides[CellSide.North] = CellSideState.Sealed;
                if (mazeState.Data[i].Any(w => w == "west")) maze[i].Sides[CellSide.West] = CellSideState.Sealed;

                if (i < width * height - 1)
                {
                    if (mazeState.Data[i + 1].Any(w => w == "west")) maze[i].Sides[CellSide.East] = CellSideState.Sealed;
                }
                else
                {
                    maze[i].Sides[CellSide.East] = CellSideState.Sealed;
                }

                if (i < width * (height - 1))
                {
                    if (mazeState.Data[i + width].Any(w => w == "north")) maze[i].Sides[CellSide.South] = CellSideState.Sealed;
                }
                else
                {
                    maze[i].Sides[CellSide.South] = CellSideState.Sealed;
                }
            }

            return maze;
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
                var mazeState = await GetMazeCurrentState(mazeId);
                if (mazeState.GameState.MazeState.ToLower().Equals("active"))
                {
                    return mazeId;
                }
            }

            mazeId = await InitializeMaze(15, 15, "Applejack", 1);
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
            var maze = new MazeInitializer
            {
                Width = width,
                Height = heigth,
                PlayerName = playerName,
                Difficulty = difficulty
            };
            return await CreateNewMazeGame(maze);
        }

        #region API Call Methods

        /// <summary>
        /// Post a request to create a new maze game and gets its id in return
        /// </summary>
        /// <param name="maze">A Maze object that specifies all of its properties</param>
        /// <returns>The Id of the maze as a string</returns>
        static async Task<string> CreateNewMazeGame(MazeInitializer maze)
        {
            var serializedMaze = JsonConvert.SerializeObject(maze);
            var content = new StringContent(serializedMaze, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"pony-challenge/maze", content);
            content.Dispose();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }
            var mazeId = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(MazeId));

            return ((MazeId)mazeId).Id;
        }

        /// <summary>
        /// Send a request to get the maze and print in the console
        /// </summary>
        /// <param name="mazeId">The maze Id</param>
        /// <returns></returns>
        static async Task PrintMaze(string mazeId)
        {
            var response = await client.GetAsync($"pony-challenge/maze/{mazeId}/print");
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }

            Console.Clear();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static async Task<Statuses> MakeNextMove(string mazeId, string move)
        {
            var serializedMove = JsonConvert.SerializeObject(new { direction = move });
            var content = new StringContent(serializedMove, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"pony-challenge/maze/{mazeId}", content);
            content.Dispose();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }
            var statuses = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(Statuses));

            return (Statuses)statuses;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mazeId"></param>
        /// <returns></returns>
        static async Task<MazeState> GetMazeCurrentState(string mazeId)
        {
            var response = await client.GetAsync($"pony-challenge/maze/{mazeId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }
            var mazeState = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(MazeState));

            return (MazeState)mazeState;
        }

        #endregion
    }
}