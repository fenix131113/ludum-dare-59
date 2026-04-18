using UnityEngine;
using VContainer;

namespace MiniGames.View
{
    public class GeneralMinigamesView : MonoBehaviour
    {
        [SerializeField] private RectTransform gamePanel;
        
        [Inject] private MinigamesManager _minigamesManager;

        private void Start() => Bind();

        private void OnDestroy() => Expose();
        
        private void OnMinigameStateChanged()
        {
            if(_minigamesManager.IsPlaying)
                ShowGameZone();
            else
                HideGameZone();
        }

        private void ShowGameZone()
        {
            gamePanel.gameObject.SetActive(true);
        }

        private void HideGameZone()
        {
            gamePanel.gameObject.SetActive(false);
        }
        
        private void Bind()
        {
            _minigamesManager.OnMinigameStateChanged += OnMinigameStateChanged;
        }

        private void Expose()
        {
            _minigamesManager.OnMinigameStateChanged -= OnMinigameStateChanged;
        }
    }
}