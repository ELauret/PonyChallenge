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

                var mazeId = await CreateNewMazeGame(maze);
                Console.WriteLine(mazeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task<string> CreateNewMazeGame(Maze maze)
        {
            var serializedMaze = JsonConvert.SerializeObject(maze);
            var content = new StringContent(serializedMaze, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"pony-challenge/maze", content);
            response.EnsureSuccessStatusCode();
            var mazeId = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(MazeId));

            return ((MazeId)mazeId).Id;
        }
    }
}
