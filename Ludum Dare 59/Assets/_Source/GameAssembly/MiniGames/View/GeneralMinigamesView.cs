using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace MiniGames.View
{
    public class GeneralMinigamesView : MonoBehaviour
    {
        [SerializeField] private RectTransform gamePanel;
        [SerializeField] private Image backgroundBlocker;
        [SerializeField] private float onOffTime = 0.15f;
        [SerializeField] private Ease onOffEase;
        [SerializeField] private float downY;
        [SerializeField] private float fadeAmount;
        
        [Inject] private MinigamesManager _minigamesManager;
        private Vector3 _startPos;

        private void Start()
        {
            Bind();
            _startPos = gamePanel.position;
        }

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
            DOTween.Kill(backgroundBlocker);
            DOTween.Kill(gamePanel);
            backgroundBlocker.gameObject.SetActive(true);
            backgroundBlocker.DOFade(fadeAmount, onOffTime);
            gamePanel.anchoredPosition = new Vector2(0, downY);
            gamePanel.gameObject.SetActive(true);
            gamePanel.DOMoveY(_startPos.y, onOffTime).SetEase(onOffEase);
        }

        private void HideGameZone()
        {
            DOTween.Kill(backgroundBlocker);
            DOTween.Kill(gamePanel);
            backgroundBlocker.DOFade(0f, onOffTime);
            gamePanel.DOMoveY(downY, onOffTime).SetEase(onOffEase).OnComplete(() =>
            {
                gamePanel.gameObject.SetActive(false);
                backgroundBlocker.gameObject.SetActive(false);
            });
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