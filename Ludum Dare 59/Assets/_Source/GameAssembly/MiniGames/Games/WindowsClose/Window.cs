using System;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace MiniGames.Games.WindowsClose
{
    public class Window : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Image contentImage;
        [SerializeField] private float closeTime;
        [SerializeField] private Sprite[] windowContentVariants;

        private bool _isBind;
        
        public event Action OnClose;

        private void Start()
        {
            Bind();
            ResetWindow();
        }

        private void OnDestroy()
        {
            Expose();
        }

        private void CloseWindow()
        {
            gameObject.SetActive(false);
            OnClose?.Invoke();
        }

        public void ResetWindow()
        {
            contentImage.sprite = windowContentVariants.GetRandomElement();
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