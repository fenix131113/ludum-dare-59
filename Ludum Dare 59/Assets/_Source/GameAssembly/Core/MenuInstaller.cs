using LevelsSystem;
using UnityEngine;
using Utils;
using VContainer;
using VContainer.Unity;

namespace Core
{
    public class MenuInstaller : LifetimeScope
    {

        [SerializeField] private SceneTransitionEffect sceneTransition;
        protected override void Configure(IContainerBuilder builder)
        {
            var levelsRecorder = FindFirstObjectByType<LevelsRecorder>();
            if (levelsRecorder)
                builder.RegisterComponent(levelsRecorder);

            builder.RegisterInstance(sceneTransition);
            builder.RegisterComponentInHierarchy<SceneLoader>();
        }
    }
}
