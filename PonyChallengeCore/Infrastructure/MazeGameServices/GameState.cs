using Newtonsoft.Json;
using PonyChallengeCore.Core.Interfaces;

namespace PonyChallengeCore.Infrastructure.MazeGameServices
{
    public class GameState : IGameState
    {
        [JsonProperty(PropertyName = "state")]
        public string MazeState { get; set; }
        [JsonProperty(PropertyName = "state-result")]
        public string MoveStatus { get; set; }
        [JsonProperty(PropertyName = "hidden-url")]
        public string HiddenUrl { get; set; }
    }
}
