using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PonyChallengeCore.Model
{
    public class MazeState
    {
        [JsonProperty(PropertyName = "pony")]
        public int[] PonyPosition { get; set; }
        [JsonProperty(PropertyName = "domokun")]
        public int[] DomokunPosition { get; set; }
        [JsonProperty(PropertyName = "end-point")]
        public int[] ExitPosition { get; set; }
        [JsonProperty(PropertyName = "size")]
        public int[] Dimensions { get; set; }
        [JsonProperty(PropertyName = "difficulty")]
        public int Difficulty { get; set; }
        [JsonProperty(PropertyName = "data")]
        public string[][] Data { get; set; }
        [JsonProperty(PropertyName = "maze_id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "game-state")]
        public Statuses GameState { get; set; }
    }
}
