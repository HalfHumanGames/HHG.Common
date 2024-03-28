using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.Common.Runtime
{
    public class UIDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        private Vector2 offset;
        private RectTransform canvasRect;
        private RectTransform rect;

        public void Start()
        {
            rect = GetComponent<RectTransform>();
            canvasRect = GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            rect.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out offset);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (rect && RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 position))
            {
                rect.localPosition = position - offset;                
                rect.ClampTransform(canvasRect);     
            }
        }
    } 
}