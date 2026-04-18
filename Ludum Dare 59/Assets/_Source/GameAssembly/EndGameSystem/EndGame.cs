using System;
using LevelsSystem;
using LevelsSystem.Data;
using TimerSystem;
using VContainer;

namespace EndGameSystem
{
    public class EndGame
    {
        private GameTimerCondition _gameTimers;
        private LevelsRecorder _levelsRecorder;
        private LevelDataSO _levelData;

        public bool IsEnded { get; private set; }

        public event Action<bool> OnGameEnded;

        [Inject]
        private void Construct(GameTimerCondition gameTimers, LevelsRecorder levelsRecorder, LevelDataSO levelDataSO)
        {
            _gameTimers = gameTimers;
            _levelsRecorder = levelsRecorder;
            _levelData = levelDataSO;
            
            Bind();
        }
        
        ~EndGame() => Expose();

        private void End(bool isWin)
        {
            if (isWin)
                _levelsRecorder.MarkLevelAsCompleted(_levelData.LevelNumber);
            
            IsEnded = true;
            OnGameEnded?.Invoke(isWin);
        }
        
        private void OnSignalTimerElapsed() => End(false);

        private void OnGameTimerElapsed() => End(true);

        private void Bind()
        {
            _gameTimers.OnGameTimerElapsed += OnGameTimerElapsed;
            _gameTimers.OnSignalTimerElapsed += OnSignalTimerElapsed;
        }

        private void Expose()
        {
            _gameTimers.OnGameTimerElapsed -= OnGameTimerElapsed;
            _gameTimers.OnSignalTimerElapsed -= OnSignalTimerElapsed;
        }
    }
}