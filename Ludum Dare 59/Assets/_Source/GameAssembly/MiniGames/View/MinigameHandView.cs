using MiniGames.Games.FilesDrag;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;

namespace MiniGames.View
{
    public class MinigameHandView : MonoBehaviour
    {
        [SerializeField] private RectTransform handRect;
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private Vector2 pointerOffset;
        [SerializeField] private Image image;
        [SerializeField] private Sprite interactCursor;
        [SerializeField] private Sprite holdCursor;
        [SerializeField] private Sprite defaultCursor;
        [SerializeField] private Sprite dragCursor;

        [Inject] private MinigamesManager _minigamesManager;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            // Bind();
            // RedrawState();
        }

        private void Update()
        {
            if (!handRect)
                return;

            MoveToPointer();

            if (CheckForButtons())
            {
                if (Mouse.current.leftButton.isPressed)
                    image.sprite = defaultCursor;
                else
                    image.sprite = interactCursor;
            }
            else if (CheckForDraggables())
            {
                if (Mouse.current.leftButton.isPressed)
                    image.sprite = holdCursor;
                else
                    image.sprite = dragCursor;
            }
            else
                image.sprite = defaultCursor;
            image.SetNativeSize();
        }

        private bool CheckForDraggables()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = Mouse.current.position.value;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult r in results)
            {
                DragFileItem dragItem = r.gameObject.GetComponentInParent<DragFileItem>();
                if (dragItem != null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckForButtons()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = Mouse.current.position.value;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult hit in results)
            {
                Button btn = hit.gameObject.GetComponentInParent<Button>();

                if (btn != null && btn.interactable)
                {
                    return true;
                }
            }
            return false;
        }

        // private void OnDestroy() => Expose();

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
