using EffectSystem;
using EndGameSystem;
using LevelsSystem;
using LevelsSystem.Data;
using MiniGames;
using Player;
using Player.Data;
using Player.Variables;
using SignalSystem;
using TimerSystem;
using UnityEngine;
using Utils;
using VContainer;
using VContainer.Unity;

namespace Core
{
    public class GameInstaller : LifetimeScope
    {
        [SerializeField] private PlayerSettingsSO playerSettings;
        [SerializeField] private LevelDataSO levelData;

        private InputSystem_Actions _inputActions;
        private static GameInstaller _instance;

        protected override void Configure(IContainerBuilder builder)
        {
            #region Core

#if UNITY_EDITOR
            var levelsRecorder = FindFirstObjectByType<LevelsRecorder>();
            if (levelsRecorder)
                builder.RegisterComponent(levelsRecorder);
#else
            builder.RegisterComponentInHierarchy<LevelsRecorder>();
#endif

            #endregion

            #region Player

            _inputActions = new InputSystem_Actions();
            _inputActions.Player.Enable();

            builder.RegisterInstance(playerSettings);
            builder.RegisterInstance(_inputActions);

            builder.Register<PlayerInput>(Lifetime.Scoped);

            builder.Register<PlayerVariables>(Lifetime.Scoped)
                .AsImplementedInterfaces()
                .AsSelf();

            #endregion

            #region Signal

            builder.RegisterComponentInHierarchy<SignalHolder>();
            builder.RegisterComponentInHierarchy<SignalTracker>();

            #endregion

            builder.Register<EffectBank>(Lifetime.Scoped)
                .As<ITickable>()
                .AsSelf();
            builder.Register<TimersHandler>(Lifetime.Scoped)
                .As<ITickable>()
                .AsSelf();
            
            builder.RegisterComponentInHierarchy<GameTimerCondition>();
            builder.RegisterComponentInHierarchy<MinigamesManager>();
            builder.Register<EndGame>(Lifetime.Scoped);
            builder.RegisterInstance(levelData);
        }

        private void Start()
        {
            ObjectInjector.Initialize(Container);
            _instance = this;
        }

        public static T Resolve<T>()
        {
            return !_instance ? default : _instance.Container.Resolve<T>();
        }
    }
}