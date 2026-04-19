using System;
using LevelsSystem;
using LevelsSystem.Data;
using TimerSystem;
using VContainer;

namespace EndGameSystem
{
    public class EndGame
    {
        private GameCondition _game;
        private LevelsRecorder _levelsRecorder;
        private LevelDataSO _levelData;

        public bool IsEnded { get; private set; }

        public event Action<bool> OnGameEnded;

        [Inject]
        private void Construct(IObjectResolver resolver, GameCondition game, LevelDataSO levelDataSO)
        {
            _game = game;
            _levelsRecorder = resolver.ResolveOrDefault<LevelsRecorder>();
            _levelData = levelDataSO;
            
            Bind();
        }
        
        ~EndGame() => Expose();

        private void End(bool isWin)
        {
            if (isWin && _levelsRecorder)
                _levelsRecorder.MarkLevelAsCompleted(_levelData.LevelNumber);
            
            IsEnded = true;
            OnGameEnded?.Invoke(isWin);
        }
        
        private void OnSignalElapsed() => End(false);

        private void OnGameElapsed() => End(false);
        private void OnSignalsReached() => End(true);

        private void Bind()
        {
            _game.OnGameTimerElapsed += OnGameElapsed;
            _game.OnSignalTimerElapsed += OnSignalElapsed;
            _game.OnSignalsReached += OnSignalsReached;
        }

        private void Expose()
        {
            _game.OnGameTimerElapsed -= OnGameElapsed;
            _game.OnSignalTimerElapsed -= OnSignalElapsed;
        }
    }
}