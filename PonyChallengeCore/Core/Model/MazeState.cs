using Newtonsoft.Json;
using PonyChallengeCore.Infrastructure.MazeGameServices;

namespace PonyChallengeCore.Model
{
    public class MazeState : MazeCreationInfo
    {
        [JsonProperty(PropertyName = "pony")]
        public int[] PonyPosition { get; set; }
        [JsonProperty(PropertyName = "domokun")]
        public int[] DomokunPosition { get; set; }
        [JsonProperty(PropertyName = "end-point")]
        public int[] ExitPosition { get; set; }
        [JsonProperty(PropertyName = "data")]
        public string[][] Data { get; set; }
        [JsonProperty(PropertyName = "maze_id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "game-state")]
        public GameState GameState { get; set; }

        int[] dimensions;
        [JsonProperty(PropertyName = "size")]
        public int[] Dimensions
        {
            get { return dimensions; }

            set
            {
                dimensions = value;
                Width = dimensions[0];
                Height = dimensions[1];
            }
        }
    }
}
