using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.Common.Runtime
{
    public class PointerListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerUpHandler
    {
        [SerializeField] private ActionEvent onPointerClick = new ActionEvent();
        [SerializeField] private ActionEvent onPointerDown = new ActionEvent();
        [SerializeField] private ActionEvent onPointerEnter = new ActionEvent();
        [SerializeField] private ActionEvent onPointerExit = new ActionEvent();
        [SerializeField] private ActionEvent onPointerMove = new ActionEvent();
        [SerializeField] private ActionEvent onPointerUp = new ActionEvent();

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick?.Invoke(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke(this);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            onPointerMove?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp?.Invoke(this);
        }
    }
}