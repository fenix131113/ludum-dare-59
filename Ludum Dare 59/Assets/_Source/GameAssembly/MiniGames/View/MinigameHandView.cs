using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace MiniGames.View
{
    public class MinigameHandView : MonoBehaviour
    {
        [SerializeField] private RectTransform handRect;
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private Vector2 pointerOffset;

        [Inject] private MinigamesManager _minigamesManager;

        private void Start()
        {
            Bind();
            RedrawState();
        }

        private void Update()
        {
            if (!_minigamesManager.IsPlaying || !handRect)
                return;

            MoveToPointer();
        }

        private void OnDestroy() => Expose();

        private void MoveToPointer()
        {
            var parentRect = GetTargetRect();

            if (!parentRect)
                return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Mouse.current.position.value, GetEventCamera(),
                    out var pointerPosition))
                return;

            handRect.anchoredPosition = pointerPosition + pointerOffset;
        }

        private RectTransform GetTargetRect()
        {
            if (rootCanvas)
                return rootCanvas.transform as RectTransform;

            if (handRect)
                return handRect.parent as RectTransform;

            return null;
        }

        private Camera GetEventCamera()
        {
            if (!rootCanvas || rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return rootCanvas.worldCamera;
        }

        private void OnMinigameStateChanged()
        {
            RedrawState();
        }

        private void RedrawState()
        {
            if (handRect)
                handRect.gameObject.SetActive(_minigamesManager.IsPlaying);
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
