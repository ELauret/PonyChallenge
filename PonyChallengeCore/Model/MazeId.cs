using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PonyChallengeCore.Model
{
    public class MazeId
    {
        [JsonProperty(PropertyName = "maze_id")]
        public string Id { get; set; }
    }
}
