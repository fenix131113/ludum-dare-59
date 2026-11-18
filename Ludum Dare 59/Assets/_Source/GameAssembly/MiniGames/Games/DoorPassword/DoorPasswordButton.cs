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
        [SerializeField] private Sprite defaultSprite;
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
                targetImage.sprite = button.spriteState.pressedSprite;
                button.enabled = false;
            }

            OnClicked?.Invoke(value);
        }
        
        public void ResetState()
        {
            if (!targetImage || !button)
                return;

            targetImage.sprite = defaultSprite;
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
