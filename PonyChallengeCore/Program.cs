using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PonyChallengeCore
{
    public class Maze
    {
        [JsonProperty(PropertyName = "maze-width")]
        public int Width { get; set; }
        [JsonProperty(PropertyName = "maze-height")]
        public int Height { get; set; }
        [JsonProperty(PropertyName = "maze-player-name")]
        public string PlayerName { get; set; }
        [JsonProperty(PropertyName = "difficulty")]
        public int Difficulty { get; set; }
    }

    public class MazeId
    {
        [JsonProperty(PropertyName = "maze_id")]
        public string Id { get; set; }
    }

    public class Statuses
    {
        [JsonProperty(PropertyName = "state")]
        public string MazeState { get; set; }
        [JsonProperty(PropertyName = "state-result")]
        public string MoveStatus { get; set; }
    }

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
                var maze = new Maze
                {
                    Width = 15,
                    Height = 15,
                    PlayerName = "Applejack",
                    Difficulty = 1
                };

                var mazeId = "154adb05-b8ac-45d4-81fb-29ec5b96a7f7";
                //var mazeId = await CreateNewMazeGame(maze);
                Console.WriteLine(mazeId);

                // Print the maze in its initial state
                await PrintMaze(mazeId);

                bool active = true;
                var validInputs = new List<string>() { "N", "W", "S", "E" };
                while (active)
                {
                    Console.WriteLine($"What is your next move (N, W, S, E)?");
                    var input = Console.ReadLine();

                    if (!validInputs.Contains(input.ToUpper()))
                    {
                        Console.WriteLine("The input is invalid.");
                        continue;
                    }

                    var move = string.Empty;
                    switch (input.ToUpper())
                    {
                        case "N":
                            move = "north";
                            break;
                        case "W":
                            move = "west";
                            break;
                        case "S":
                            move = "south";
                            break;
                        case "E":
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
        static async Task<string> CreateNewMazeGame(Maze maze)
        {
            var serializedMaze = JsonConvert.SerializeObject(maze);
            var content = new StringContent(serializedMaze, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"pony-challenge/maze", content);
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
            response.EnsureSuccessStatusCode();

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
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }
            var statuses = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(Statuses));

            return (Statuses)statuses;
        }
    }
}
