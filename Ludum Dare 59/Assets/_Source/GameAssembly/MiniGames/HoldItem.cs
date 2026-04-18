using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGames
{
    public class HoldItem : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private RectTransform limitRect;
        [SerializeField] private Canvas canvas;

        private Vector3 _currentOffset;
        private bool _isBlocked;
        private readonly Vector3[] _rectCorners = new Vector3[4];
        private readonly Vector3[] _limitCorners = new Vector3[4];

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isBlocked)
                return;

            if (!TryGetPointerWorldPosition(eventData, out var pointerWorldPosition))
                return;

            _currentOffset = rect.position - pointerWorldPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isBlocked)
                return;

            if (!TryGetPointerWorldPosition(eventData, out var pointerWorldPosition))
                return;

            var targetWorldPosition = pointerWorldPosition + _currentOffset;
            rect.position = new Vector3(targetWorldPosition.x, targetWorldPosition.y, rect.position.z);

            ClampInsideLimitRect();
        }

        private bool TryGetPointerWorldPosition(PointerEventData eventData, out Vector3 pointerWorldPosition)
        {
            var parentRect = rect.parent as RectTransform;
            
            if (parentRect)
                return RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, eventData.position,
                    GetEventCamera(eventData),
                    out pointerWorldPosition);
            
            pointerWorldPosition = default;
            return false;
        }
        
        private Camera GetEventCamera(PointerEventData eventData)
        {
            if(eventData.pressEventCamera)
                return eventData.pressEventCamera;

            if(eventData.enterEventCamera)
                return eventData.enterEventCamera;

            var parentCanvas = rect.GetComponentInParent<Canvas>();
            if(!parentCanvas || parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return parentCanvas.worldCamera;
        }

        private void ClampInsideLimitRect()
        {
            var bounds = GetLimitRect();
            
            if (!bounds)
                return;

            bounds.GetWorldCorners(_limitCorners);
            rect.GetWorldCorners(_rectCorners);

            var minLimitX = Mathf.Min(_limitCorners[0].x, _limitCorners[2].x);
            var maxLimitX = Mathf.Max(_limitCorners[0].x, _limitCorners[2].x);
            var minLimitY = Mathf.Min(_limitCorners[0].y, _limitCorners[2].y);
            var maxLimitY = Mathf.Max(_limitCorners[0].y, _limitCorners[2].y);

            var minRectX = Mathf.Min(_rectCorners[0].x, _rectCorners[2].x);
            var maxRectX = Mathf.Max(_rectCorners[0].x, _rectCorners[2].x);
            var minRectY = Mathf.Min(_rectCorners[0].y, _rectCorners[2].y);
            var maxRectY = Mathf.Max(_rectCorners[0].y, _rectCorners[2].y);

            var deltaX = CalculateDelta(minRectX, maxRectX, minLimitX, maxLimitX);
            var deltaY = CalculateDelta(minRectY, maxRectY, minLimitY, maxLimitY);

            if (Mathf.Approximately(deltaX, 0f) && Mathf.Approximately(deltaY, 0f))
                return;

            rect.position += new Vector3(deltaX, deltaY, 0f);
        }

        private float CalculateDelta(float minValue, float maxValue, float minLimit, float maxLimit)
        {
            var valueSize = maxValue - minValue;
            var limitSize = maxLimit - minLimit;

            if (valueSize > limitSize)
            {
                var valueCenter = (minValue + maxValue) * 0.5f;
                var limitCenter = (minLimit + maxLimit) * 0.5f;
                return limitCenter - valueCenter;
            }

            if (minValue < minLimit)
                return minLimit - minValue;

            if (maxValue > maxLimit)
                return maxLimit - maxValue;

            return 0f;
        }

        private RectTransform GetLimitRect()
        {
            if (limitRect)
                return limitRect;

            return canvas ? canvas.transform as RectTransform : null;
        }

        public void BlockItem() => _isBlocked = true;

        public void UnblockItem() => _isBlocked = false;
    }
}