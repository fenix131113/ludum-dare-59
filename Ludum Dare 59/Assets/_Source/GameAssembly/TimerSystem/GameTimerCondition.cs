using System;
using SignalSystem;
using UnityEngine;
using VContainer;

namespace TimerSystem
{
    public class GameTimerCondition : MonoBehaviour
    {
        [SerializeField] private float gameTime;
        [SerializeField] private float signalTime;
        
        [Inject] private TimersHandler _timers;
        [Inject] private SignalHolder _signalHolder;

        private Timer _gameTimer;
        private Timer _signalTimer;

        public event Action OnGameTimerElapsed;
        public event Action OnSignalTimerElapsed;
        
        private void Start()
        {
            Bind();
            StartGameTimer();
            
            if(_signalHolder.GetCurrentSignal())
                StartSignalTimer();
        }

        private void OnDestroy()
        {
            Expose();
        }

        private void StartGameTimer()
        {
            _gameTimer = new Timer(gameTime);
            _timers.RegisterTimer(_gameTimer);
            BindGameTimer();
        }

        private void StartSignalTimer()
        {
            if(_signalTimer != null)
            {
                _timers.UnregisterTimer(_signalTimer.ID);
                _signalTimer.OnTimeElapsed -= SignalTimerElapsed;
            }
             
            _signalTimer = new Timer(signalTime);
            _timers.RegisterTimer(_signalTimer);
            BindSignalTimer();
        }

        private void GameTimerElapsed(Timer _)
        {
            OnGameTimerElapsed?.Invoke();
        }
        
        private void SignalTimerElapsed(Timer _)
        {
            _gameTimer.Pause();
            OnSignalTimerElapsed?.Invoke();
        }
        
        private void OnSignalSpawned()
        {
            StartSignalTimer();
        }
        
        public Timer GetGameTimer() => _gameTimer;
        public Timer GetSignalTimer() => _signalTimer;

        private void Bind()
        {
            _signalHolder.OnSignalSpawned += OnSignalSpawned;
        }

        private void BindGameTimer()
        {
            _gameTimer.OnTimeElapsed += GameTimerElapsed;
        }

        private void BindSignalTimer()
        {
            _signalTimer.OnTimeElapsed += SignalTimerElapsed;
        }

        private void Expose()
        {
            if(_gameTimer != null)
                _gameTimer.OnTimeElapsed -= GameTimerElapsed;
            
            if(_signalTimer != null)
                _signalTimer.OnTimeElapsed -= SignalTimerElapsed;
            
            _signalHolder.OnSignalSpawned -= OnSignalSpawned;
        }
    }
}
