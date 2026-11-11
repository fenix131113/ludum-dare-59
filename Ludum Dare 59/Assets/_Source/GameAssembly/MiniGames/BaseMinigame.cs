using System;
using UnityEngine;

namespace MiniGames
{
    public abstract class BaseMinigame : MonoBehaviour, IMinigame
    {
        public event Action GameEnded;
        public event Action GameExited;
        public abstract void StartMinigame();
        public abstract void EndMinigame();
        public abstract void ResetGame();
        
        protected void InvokeGameEnded() =>  GameEnded?.Invoke();
        protected void InvokeGameExited() =>  GameExited?.Invoke();
    }
}