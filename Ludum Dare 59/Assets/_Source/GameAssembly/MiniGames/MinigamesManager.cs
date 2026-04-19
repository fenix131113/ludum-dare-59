using System;
using Player.Data;
using Player.Variables;
using SignalSystem;
using UnityEngine;
using Utils.VariablesSystem;
using VContainer;

namespace MiniGames
{
    public class MinigamesManager : MonoBehaviour
    {
        [Inject] private IVariablesResolver<PlayerVariableBlockerType, Action, Action> _variablesResolver;

        private IMinigame _currentMinigame;
        private SignalEmitter _currentSignalEmitter;
        private readonly PlayerVariableBlocker _minigameBlocker = new(PlayerVariableBlockerType.ALL);

        public bool IsPlaying => _currentMinigame != null;

        public event Action OnMinigameStateChanged;

        public IMinigame GetCurrentMinigame() => _currentMinigame;

        public void PlayMinigame(IMinigame minigame, SignalEmitter signalEmitter)
        {
            if (_currentMinigame != null)
                return;

            _currentMinigame = minigame;
            _currentSignalEmitter = signalEmitter;
            _currentMinigame.GameEnded += EndMinigame;
            _currentMinigame.StartMinigame();
            _variablesResolver?.RegisterBlocker(_minigameBlocker);
            OnMinigameStateChanged?.Invoke();
        }

        public void EndMinigame()
        {
            if (_currentMinigame == null)
                return;
            
            _currentMinigame.EndMinigame();
            _currentMinigame.GameEnded -= EndMinigame;
            _currentMinigame = null;
            _minigameBlocker.Dispose();
            _currentSignalEmitter?.SendSignal();
            _currentSignalEmitter = null;
            OnMinigameStateChanged?.Invoke();
        }
    }
}