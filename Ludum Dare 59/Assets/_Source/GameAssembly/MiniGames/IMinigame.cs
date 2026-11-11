using System;

namespace MiniGames
{
    public interface IMinigame
    {
        event Action GameEnded;
        event Action GameExited;
        void StartMinigame();
        void EndMinigame();
        void ResetGame();
    }
}