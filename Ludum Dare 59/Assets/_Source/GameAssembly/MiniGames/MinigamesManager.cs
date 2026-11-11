using System;
using Player;
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
        [Inject] private PlayerInput _playerInput;

        private IMinigame _currentMinigame;
        private SignalEmitter _currentSignalEmitter;
        private readonly PlayerVariableBlocker _minigameBlocker = new(PlayerVariableBlockerType.ALL);

        public bool IsPlaying => _currentMinigame != null;

        public bool IsDoorMinigame => _currentSignalEmitter == null;

        public event Action OnMinigameStateChanged;
        public event Action OnMinigameExited;

        public IMinigame GetCurrentMinigame() => _currentMinigame;

        public void PlayMinigame(IMinigame minigame, SignalEmitter signalEmitter)
        {
            if (_currentMinigame != null)
                return;

            _currentMinigame = minigame;
            _currentSignalEmitter = signalEmitter;
            _currentMinigame.GameEnded += EndMinigame;
            _playerInput.OnPauseClicked += ExitFromMinigame;
            _currentMinigame.StartMinigame();
            _variablesResolver?.RegisterBlocker(_minigameBlocker);
            OnMinigameStateChanged?.Invoke();
        }
        
        public void PlayMinigame(IMinigame minigame) => PlayMinigame(minigame, null);

        public void EndMinigame()
        {
            if (_currentMinigame == null)
                return;
            
            _currentMinigame.EndMinigame();
            _currentMinigame.GameEnded -= EndMinigame;
            _playerInput.OnPauseClicked -= ExitFromMinigame;
            _currentMinigame = null;
            _minigameBlocker.Dispose();
            _currentSignalEmitter?.SendSignal();
            _currentSignalEmitter = null;
            OnMinigameStateChanged?.Invoke();
        }

        public void ExitFromMinigame()
        {
            if (_currentMinigame == null)
                return;
            if (!IsDoorMinigame) return;

            _currentMinigame.EndMinigame();
            _currentMinigame.GameEnded -= EndMinigame;
            _playerInput.OnPauseClicked -= ExitFromMinigame;
            _currentMinigame = null;
            _minigameBlocker.Dispose();
            _currentSignalEmitter = null;
            OnMinigameExited?.Invoke();
        }
    }
}