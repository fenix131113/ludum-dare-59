using EffectSystem;
using LevelsSystem;
using Player;
using Player.Data;
using Player.Variables;
using SignalSystem;
using UnityEngine;
using Utils;
using VContainer;
using VContainer.Unity;

namespace Core
{
    public class GameInstaller : LifetimeScope
    {
        [SerializeField] private PlayerSettingsSO playerSettings;

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

            #region Effects

            builder.Register<EffectBank>(Lifetime.Scoped)
                .As<ITickable>()
                .AsSelf();

            #endregion
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