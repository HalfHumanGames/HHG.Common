using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.Common.Runtime
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, /*IPointerMoveHandler,*/ IBeginDragHandler, /*IDragHandler,*/ IEndDragHandler
    {
        //public bool ShowOnDrag
        //{
        //    get => showOnDrag;
        //    set => showOnDrag = value;
        //}

        //[SerializeField] private bool showOnDrag;

        private UITooltip ui;
        private bool isDragging;

        private void Start()
        {
            ui = Locator.Get<UITooltip>();
        }

        private void OnMouseEnter()
        {
            ShowTooltip();
        }

        private void OnMouseExit()
        {
            HideTooltip();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isDragging = eventData.pointerDrag;
            ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isDragging = eventData.pointerDrag;
            HideTooltip();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            //isDragging = eventData.pointerDrag;
            //HideTooltip();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = eventData.pointerDrag;
            HideTooltip();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = eventData.pointerDrag;
            HideTooltip();
        }

        public void OnDrag(PointerEventData eventData)
        {
            //isDragging = eventData.pointerDrag;
            //HideTooltip();
        }

        public void ShowTooltip()
        {
            //if (ui && (showOnDrag || !isDragging) && gameObject.TryGetComponent(out ITooltip tooltip))
            if (ui && !isDragging && gameObject.TryGetComponent(out ITooltip tooltip))
            {
                ui.Show(tooltip);
            }
        }

        public void HideTooltip()
        {
            if (ui)
            {
                ui.Hide();
            }
        }
    } 
}