using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PonyChallengeCore.Model;

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
