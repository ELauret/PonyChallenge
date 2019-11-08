using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PonyChallengeCore.Model;

namespace PonyChallengeCore.Core.Interfaces
{
    interface IMazeGameService
    {
        Task<string> CreateNewMazeGameAsync(MazeCreationInfo maze);
        Task PrintMazeAsync(string mazeId);
        Task<GameState> MakeNextMoveAsync(string mazeId, string move);
        Task<MazeState> GetMazeCurrentStateAsync(string mazeId);
    }
}
