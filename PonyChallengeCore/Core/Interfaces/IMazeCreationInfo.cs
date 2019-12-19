namespace PonyChallengeCore.Core.Interfaces
{
    public interface IMazeCreationInfo
    {
        int Width { get; set; }
        int Height { get; set; }
        string PlayerName { get; set; }
        int Difficulty { get; set; }
    }
}
