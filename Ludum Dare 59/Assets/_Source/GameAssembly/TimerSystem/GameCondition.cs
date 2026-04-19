using System;
using SignalSystem;
using UnityEngine;
using VContainer;

namespace TimerSystem
{
    public class GameCondition : MonoBehaviour
    {
        [SerializeField] private float gameTime;
        [SerializeField] private float signalTime;
        [SerializeField] private int needSignalToWin;
        
        [Inject] private TimersHandler _timers;
        [Inject] private SignalHolder _signalHolder;

        private Timer _gameTimer;
        private Timer _signalTimer;
        private int _currentSignals;

        public event Action OnGameTimerElapsed;
        public event Action OnSignalTimerElapsed;
        public event Action OnCurrentSignalsChanged;
        public event Action OnSignalsReached;
        
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
            _signalTimer?.Pause();
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

        private void OnSignalSent()
        {
            _currentSignals++;
            OnCurrentSignalsChanged?.Invoke();
            
            if(_currentSignals < needSignalToWin)
                return;
            
            _gameTimer.Pause();
            _signalTimer.Pause();
            OnSignalsReached?.Invoke();
        }
        
        public Timer GetGameTimer() => _gameTimer;
        public Timer GetSignalTimer() => _signalTimer;
        public int GetCurrentSignalsCount() => _currentSignals;
        public int GetNeedSignalsCount() => needSignalToWin;

        private void Bind()
        {
            _signalHolder.OnSignalSpawned += OnSignalSpawned;
            _signalHolder.OnSignalSent += OnSignalSent;
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
            _signalHolder.OnSignalSent -= OnSignalSent;
        }
    }
}
