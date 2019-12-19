using PonyChallengeCore.Model;
using System.Threading.Tasks;

namespace PonyChallengeCore.Core.Interfaces
{
    interface IMazeGameService
    {
        Task<string> CreateNewMazeGameAsync(IMazeCreationInfo maze);
        Task PrintMazeAsync(string mazeId);
        Task<IGameState> MakeNextMoveAsync(string mazeId, string move);
        Task<MazeState> GetMazeCurrentStateAsync(string mazeId);
    }
}
