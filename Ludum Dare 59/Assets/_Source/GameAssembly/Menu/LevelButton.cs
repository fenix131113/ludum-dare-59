using UnityEngine;
using UnityEngine.UI;
using Utils;
using VContainer;

namespace Menu
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private int levelIndex;

        [Inject] private SceneLoader _sceneLoader;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void OnButtonClicked() => _sceneLoader.LoadScene(levelIndex);

        private void Bind() => button.onClick.AddListener(OnButtonClicked);

        private void Expose() => button.onClick.RemoveAllListeners();
    }
}