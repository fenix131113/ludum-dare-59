using System;

namespace MiniGames
{
    public interface IMinigame
    {
        event Action GameEnded;
        void StartMinigame();
        void EndMinigame();
        void ResetGame();
    }
}