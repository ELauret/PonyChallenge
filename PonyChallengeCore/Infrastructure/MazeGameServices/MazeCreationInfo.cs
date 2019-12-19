using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PonyChallengeCore.Core.Interfaces;

namespace PonyChallengeCore.Infrastructure.MazeGameServices
{
    public class MazeCreationInfo : IMazeCreationInfo
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
}
