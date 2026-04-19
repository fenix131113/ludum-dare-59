using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace MiniGames.Games.WindowsClose
{
    public class Window : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform windowRect;
        [SerializeField] private Image contentImage;
        [SerializeField] private float closeTime;
        [SerializeField] private Sprite[] windowContentVariants;

        private bool _isBind;
        private Vector3 _startScale;
        private Tween _currentTween;
        
        public event Action OnClose;

        private void Start()
        {
            _startScale =  windowRect.localScale;
            Bind();
            ResetWindow();
        }

        private void OnDestroy()
        {
            Expose();
        }

        private void CloseWindow()
        {
            closeButton.interactable = false;

            _currentTween?.Kill();
            _currentTween = windowRect.DOScale(Vector3.zero, closeTime).OnComplete(() =>
            {
                gameObject.SetActive(false);
                OnClose?.Invoke();
            });
        }

        public void ResetWindow()
        {
            if(_startScale == Vector3.zero)
                _startScale =  windowRect.localScale;
            
            contentImage.sprite = windowContentVariants.GetRandomElement();
            windowRect.localScale = _startScale;
            closeButton.interactable = true;
        }

        private void Bind()
        {
            if(_isBind)
                return;
            
            closeButton.onClick.AddListener(CloseWindow);
            _isBind = true;
        }

        private void Expose()
        {
            if(!_isBind)
                return;
            
            closeButton.onClick.RemoveAllListeners();
        }
    }
}