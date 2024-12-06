using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.Common.Runtime
{
    public class EventListener :
        MonoBehaviour,
        IBeginDragHandler,
        ICancelHandler,
        IDeselectHandler,
        IDragHandler,
        IDropHandler,
        IEndDragHandler,
        IInitializePotentialDragHandler,
        IMoveHandler,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerMoveHandler,
        IPointerUpHandler,
        IScrollHandler,
        ISelectHandler,
        ISubmitHandler,
        IUpdateSelectedHandler
    {
        [SerializeField, Unfold] private ActionEvent onBeginDrag = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onCancel = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onDeselect = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onDrag = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onDrop = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onEndDrag = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onInitializePotentialDrag = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onMove = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onPointerClick = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onPointerDown = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onPointerEnter = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onPointerExit = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onPointerMove = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onPointerUp = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onScroll = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onSelect = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onSubmit = new ActionEvent();
        [SerializeField, Unfold] private ActionEvent onUpdateSelected = new ActionEvent();

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.Invoke(this);
        }

        public void OnCancel(BaseEventData eventData)
        {
            onCancel?.Invoke(this);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            onDeselect?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            onDrop?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke(this);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            onInitializePotentialDrag?.Invoke(this);
        }

        public void OnMove(AxisEventData eventData)
        {
            onMove?.Invoke(this);
        }

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

        public void OnScroll(PointerEventData eventData)
        {
            onScroll?.Invoke(this);
        }

        public void OnSelect(BaseEventData eventData)
        {
            onSelect?.Invoke(this);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            onSubmit?.Invoke(this);
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            onUpdateSelected?.Invoke(this);
        }
    }
}
