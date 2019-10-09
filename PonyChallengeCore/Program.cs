using System;
using System.Net.Http;
using System.Net.Http.Headers;
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

                var mazeId = "cfe6bef4-bd4c-4ae8-a9ba-8f98532db1e6"; // await CreateNewMazeGame(maze);
                Console.WriteLine(mazeId);

                // Print the maze in its initial state
                var mazePrint = await PrintMaze(mazeId);
                Console.WriteLine(mazePrint);
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
            var content = new StringContent(serializedMaze, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"pony-challenge/maze", content);
            response.EnsureSuccessStatusCode();
            var mazeId = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(MazeId));

            return ((MazeId)mazeId).Id;
        }

        /// <summary>
        /// Send a request to get the maze and its state as a string that can be directly printed in the console
        /// </summary>
        /// <param name="mazeId">The maze Id</param>
        /// <returns>A string representing the maze and its state</returns>
        static async Task<string> PrintMaze(string mazeId)
        {
            var response = await client.GetAsync($"pony-challenge/maze/{mazeId}/print");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
