using Newtonsoft.Json;
using PonyChallengeCore.Core.Interfaces;
using PonyChallengeCore.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PonyChallengeCore.Infrastructure.MazeGameServices
{
    public class HttpMazeGameService : IMazeGameService
    {
        static HttpClient client = new HttpClient();

        public HttpMazeGameService()
        {
            client.BaseAddress = new Uri($"https://ponychallenge.trustpilot.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Post a request to create a new maze game and gets its id in return
        /// </summary>
        /// <param name="maze">A Maze object that specifies all of its properties</param>
        /// <returns>The Id of the maze as a string</returns>
        public async Task<string> CreateNewMazeGameAsync(IMazeCreationInfo maze)
        {
            var serializedMaze = JsonConvert.SerializeObject(maze);
            var content = new StringContent(serializedMaze, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"pony-challenge/maze", content);
            content.Dispose();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }
            var mazeState = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(MazeState));

            return ((MazeState)mazeState).Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mazeId"></param>
        /// <returns></returns>
        public async Task<MazeState> GetMazeCurrentStateAsync(string mazeId)
        {
            var response = await client.GetAsync($"pony-challenge/maze/{mazeId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }
            var mazeState = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(MazeState));

            return (MazeState)mazeState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mazeId"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public async Task<IGameState> MakeNextMoveAsync(string mazeId, string move)
        {
            var serializedMove = JsonConvert.SerializeObject(new { direction = move });
            var content = new StringContent(serializedMove, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"pony-challenge/maze/{mazeId}", content);
            content.Dispose();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }
            var statuses = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(GameState));

            return (GameState)statuses;
        }

        /// <summary>
        /// Send a request to get the maze and print in the console
        /// </summary>
        /// <param name="mazeId">The maze Id</param>
        /// <returns></returns>
        public async Task PrintMazeAsync(string mazeId)
        {
            var response = await client.GetAsync($"pony-challenge/maze/{mazeId}/print");
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
            }

            Console.Clear();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}
