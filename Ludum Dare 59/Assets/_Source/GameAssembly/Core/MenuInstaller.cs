using LevelsSystem;
using Utils;
using VContainer;
using VContainer.Unity;

namespace Core
{
    public class MenuInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
#if UNITY_EDITOR
            var levelsRecorder = FindFirstObjectByType<LevelsRecorder>();
            if (levelsRecorder)
                builder.RegisterComponent(levelsRecorder);
#else
            builder.RegisterComponentInHierarchy<LevelsRecorder>();
#endif

            builder.RegisterComponentInHierarchy<SceneLoader>();
        }
    }
}
