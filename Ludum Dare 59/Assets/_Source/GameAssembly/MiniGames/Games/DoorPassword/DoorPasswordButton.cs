using System;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Games.DoorPassword
{
    public class DoorPasswordButton : MonoBehaviour
    {
        [SerializeField] private int value;
        [SerializeField] private Button button;
        [SerializeField] private Image targetImage;
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color pressedColor;

        private bool _isBind;

        public int Value => value;

        public event Action<int> OnClicked;

        private void Start()
        {
            Bind();
            ResetState();
        }

        private void OnDestroy() => Expose();

        private void OnButtonClicked()
        {
            if (targetImage && button)
            {
                targetImage.color = pressedColor;
                button.enabled = false;
            }

            OnClicked?.Invoke(value);
        }
        
        public void ResetState()
        {
            if (!targetImage || !button)
                return;
            
            targetImage.color = defaultColor;
            button.enabled = true;
        }

        public void SetInteractable(bool isInteractable)
        {
            if (button)
                button.enabled = isInteractable;
        }

        private void Bind()
        {
            if (_isBind || !button)
                return;

            button.onClick.AddListener(OnButtonClicked);
            _isBind = true;
        }

        private void Expose()
        {
            if (!_isBind || !button)
                return;

            button.onClick.RemoveAllListeners();
            _isBind = false;
        }
    }
}
