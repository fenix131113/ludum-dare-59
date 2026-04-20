using System;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Games.FingerPrintGame
{
    public class FingerButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image buttonImage;

        public int Index { get; private set; }

        public event Action<FingerButton> OnClick;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void OnButtonClicked()
        {
            OnClick?.Invoke(this);
        }

        public void Initialize(int index, Sprite sprite)
        {
            buttonImage.sprite = sprite;
            Index = index;
        }

        public void SetInteractable(bool interactable) => button.interactable = interactable;

        private void Bind() => button.onClick.AddListener(OnButtonClicked);

        private void Expose() => button.onClick.RemoveAllListeners();
    }
}