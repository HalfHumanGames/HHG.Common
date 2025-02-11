using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.Common.Runtime
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
    {
        private UITooltip ui;
        private Lazy<ITooltip> _tooltip = new Lazy<ITooltip>();
        private ITooltip tooltip => _tooltip.FromComponent(this);
        private bool isDragging;

        private void Start()
        {
            ui = Locator.Get<UITooltip>();
        }

        private void OnDisable()
        {
            HideTooltip();
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

        public void ShowTooltip()
        {
            if (ui && tooltip != null && !isDragging)
            {
                ui.Show(tooltip);
            }
        }

        public void HideTooltip()
        {
            if (ui && tooltip != null && ui.Current == tooltip)
            {
                ui.Hide();
            }
        }
    } 
}