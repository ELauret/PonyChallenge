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

namespace PonyChallengeCore
{
    class Program
    {
        static HttpClient client = new HttpClient();

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
                var maze = new MazeInitializer
                {
                    Width = 15,
                    Height = 15,
                    PlayerName = "Applejack",
                    Difficulty = 1
                };

                var mazeId = string.Empty;
                const string fileName = "CurrentMaze.txt";
                if (File.Exists(fileName))
                {
                    mazeId = File.ReadAllText(fileName);
                }
                else
                {
                    mazeId = await CreateNewMazeGame(maze);
                    File.WriteAllText(fileName, mazeId);
                }

                //var mazeId = "154adb05-b8ac-45d4-81fb-29ec5b96a7f7";
                Console.WriteLine(mazeId);

                // Print the maze in its initial state
                await PrintMaze(mazeId);

                bool active = true;
                var validKeys = new List<ConsoleKey>()
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

                while (active)
                {
                    Console.WriteLine($"What is your next move (N, W, S, E)?");
                    var inputKey = Console.ReadKey().Key;

                    if (!validKeys.Contains(inputKey))
                    {
                        Console.WriteLine("The input is invalid.");
                        continue;
                    }

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
                            break;
                    }

                    var statuses = await MakeNextMove(mazeId, move);
                    await PrintMaze(mazeId);
                    Console.WriteLine(statuses.MoveStatus);

                    if (!statuses.MazeState.Equals("active"))
                    {
                        active = false;
                        Console.WriteLine(statuses.MazeState);
                    }

                    var mazeState = GetMazeCurrentState(mazeId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

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
    }
}