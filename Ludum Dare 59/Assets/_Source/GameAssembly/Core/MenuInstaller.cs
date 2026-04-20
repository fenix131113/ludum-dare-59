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
            var levelsRecorder = FindFirstObjectByType<LevelsRecorder>();
            if (levelsRecorder)
                builder.RegisterComponent(levelsRecorder);

            builder.RegisterComponentInHierarchy<SceneLoader>();
        }
    }
}
