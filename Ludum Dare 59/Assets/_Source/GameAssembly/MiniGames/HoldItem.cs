using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGames
{
    public class HoldItem : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private Canvas canvas;

        private Vector2 _currentOffset;
        private bool _isBlocked;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(_isBlocked)
                return;
            
            _currentOffset = (Vector2)rect.position - eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(_isBlocked)
                return;
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform,
                new Vector3(Screen.width, Screen.height), null, out var topRight);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform,
                new Vector3(0, 0), null, out var downLeft);

            var newPos = eventData.position + _currentOffset;
            rect.position = new Vector3(Mathf.Clamp(newPos.x, downLeft.x, topRight.x),
                Mathf.Clamp(newPos.y, downLeft.y, topRight.y), 0);
        }

        public void BlockItem() => _isBlocked = true;
        
        public void UnblockItem() => _isBlocked = false;
    }
}