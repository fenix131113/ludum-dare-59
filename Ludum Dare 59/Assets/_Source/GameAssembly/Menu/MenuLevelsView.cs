using System.Collections.Generic;
using LevelsSystem;
using UnityEngine;
using VContainer;

namespace Menu
{
    public class MenuLevelsView : MonoBehaviour
    {
        [SerializeField] private List<LevelButton> levelsButtons = new();

        private LevelsRecorder _levelsRecorder;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _levelsRecorder = resolver.ResolveOrDefault<LevelsRecorder>();
        }

        private void Start()
        {
            CheckLevels();
        }

        private void CheckLevels()
        {
            if (!_levelsRecorder)
                return;
            
            for (var index = 1; index < levelsButtons.Count; index++)
            {
                var button = levelsButtons[index];
                button.SetInteractable(_levelsRecorder.IsLevelCompleted(levelsButtons[index - 1].GetLevelIndex()));
            }
        }
    }
}
