using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UITooltip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private float padding;
        [SerializeField] private Vector2 offset;

        private RectTransform rect;
        private RectTransform canvasRect;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private ITooltip tooltip;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            canvasRect = canvas.GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            Locator.Register(this);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (tooltip != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Mouse.current.position.value, canvas.worldCamera, out var position))
            {
                rect.position = canvasRect.TransformPoint(position + offset);
                rect.ClampTransform(canvasRect, padding);
            }
        }

        public void Show(ITooltip tip)
        {
            tooltip = tip;
            text.text = tooltip.TooltipText;
            rect.SetAsLastSibling();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            tooltip = null;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Locator.Unregister(this);
        }
    }
}