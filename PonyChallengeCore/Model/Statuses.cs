using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PonyChallengeCore.Model
{
    public class Statuses
    {
        [JsonProperty(PropertyName = "state")]
        public string MazeState { get; set; }
        [JsonProperty(PropertyName = "state-result")]
        public string MoveStatus { get; set; }
        [JsonProperty(PropertyName = "hidden-url")]
        public string HiddenUrl { get; set; }
    }
}
