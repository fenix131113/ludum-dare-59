using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGames.Games.FilesDrag
{
    public class DragFileItem : MonoBehaviour, IEndDragHandler
    {
        [SerializeField] private HoldItem holdItem;
        [SerializeField] private RectTransform rect;
        [SerializeField] private Canvas canvas;

        private Vector2 _startLocalPosition;
        private RectTransform _dropZone;
        private bool _isDropped;
        private bool _initialized;
        private readonly Vector3[] _rectCorners = new Vector3[4];
        private readonly Vector3[] _dropZoneCorners = new Vector3[4];

        public event Action<DragFileItem> OnFileDropped;

        public void SetDropZone(RectTransform dropZone) => _dropZone = dropZone;

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isDropped || !_dropZone || !rect)
                return;

            if (!IsOverDropZone())
                return;

            _isDropped = true;
            holdItem?.BlockItem();
            gameObject.SetActive(false);
            OnFileDropped?.Invoke(this);
        }

        private bool IsOverDropZone()
        {
            if (!rect || !_dropZone)
                return false;

            rect.GetWorldCorners(_rectCorners);
            _dropZone.GetWorldCorners(_dropZoneCorners);

            var rectMinX = Mathf.Min(_rectCorners[0].x, _rectCorners[2].x);
            var rectMaxX = Mathf.Max(_rectCorners[0].x, _rectCorners[2].x);
            var rectMinY = Mathf.Min(_rectCorners[0].y, _rectCorners[2].y);
            var rectMaxY = Mathf.Max(_rectCorners[0].y, _rectCorners[2].y);

            var dropZoneMinX = Mathf.Min(_dropZoneCorners[0].x, _dropZoneCorners[2].x);
            var dropZoneMaxX = Mathf.Max(_dropZoneCorners[0].x, _dropZoneCorners[2].x);
            var dropZoneMinY = Mathf.Min(_dropZoneCorners[0].y, _dropZoneCorners[2].y);
            var dropZoneMaxY = Mathf.Max(_dropZoneCorners[0].y, _dropZoneCorners[2].y);

            return rectMaxX >= dropZoneMinX && rectMinX <= dropZoneMaxX &&
                   rectMaxY >= dropZoneMinY && rectMinY <= dropZoneMaxY;
        }

        public void ResetItem()
        {
            if(!_initialized && rect)
            {
                _startLocalPosition = rect.localPosition;
                _initialized = true;
            }

            _isDropped = false;
            holdItem?.UnblockItem();

            if (rect)
                rect.localPosition = _startLocalPosition;

            gameObject.SetActive(true);
        }
    }
}
