using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UITooltip : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("text")] private TextMeshProUGUI label;
        [SerializeField] private float padding;
        [SerializeField] private Vector2 offset;

        private RectTransform rect;
        private RectTransform canvasRect;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private string tooltipText;

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
            if (!string.IsNullOrEmpty(tooltipText))
            {
                // Only follow mouse if the cursor is visible
                if (Cursor.visible && Mouse.current != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Mouse.current.position.value, canvas.worldCamera, out Vector2 position))
                {
                    rect.position = canvasRect.TransformPoint(position + offset);
                    rect.ClampTransform(canvasRect, padding);
                }
            }
        }

        public void Show(ITooltip tooltip)
        {
            Show(tooltip.TooltipText, tooltip.TooltipPosition);
        }

        public void Show(string text, Vector3 position)
        {
            tooltipText = text;

            // Don't show if null or empty tooltip text
            if (!string.IsNullOrEmpty(tooltipText))
            {
                label.text = tooltipText;
                rect.SetAsLastSibling();
                gameObject.SetActive(true);

                // Do after game object is set active
                // Otherwise rebuild will not work
                rect.RebuildLayout();

                rect.position = position;
                rect.anchoredPosition += offset;
                rect.ClampTransform(canvasRect, padding);
            }
        }

        public void Hide()
        {
            tooltipText = null;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Locator.Unregister(this);
        }
    }
}