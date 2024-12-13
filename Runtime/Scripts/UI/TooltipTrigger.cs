using UnityEngine;
using UnityEngine.EventSystems;

namespace HHG.Common.Runtime
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private UITooltip ui;

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
            if (!eventData.pointerDrag)
            {
                ShowTooltip();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideTooltip();
        }

        public void ShowTooltip()
        {
            if (ui && gameObject.TryGetComponent(out ITooltip tooltip))
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