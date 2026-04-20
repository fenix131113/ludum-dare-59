using System.Linq;
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

            var levelsRecorder = FindFirstObjectByType<LevelsRecorder>();
            if (levelsRecorder)
                builder.RegisterComponent(levelsRecorder);

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
            
            builder.RegisterComponentInHierarchy<GameCondition>();
            builder.RegisterComponentInHierarchy<MinigamesManager>();
            builder.Register<EndGame>(Lifetime.Scoped);
            builder.RegisterInstance(levelData);
            
            var minigames = FindObjectsByType<BaseMinigame>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            builder.RegisterInstance(minigames.ToArray());
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
