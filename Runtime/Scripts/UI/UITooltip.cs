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
            if (tooltip != null)
            {
                // Only follow mouse if the cursor is visible
                if (Cursor.visible && Mouse.current != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Mouse.current.position.value, canvas.worldCamera, out Vector2 position))
                {
                    rect.position = canvasRect.TransformPoint(position + offset);
                    rect.ClampTransform(canvasRect, padding);
                }
            }
        }

        public void Show(ITooltip nextTooltip)
        {
            tooltip = nextTooltip;

            // Don't show if null or empty tooltip text
            if (!string.IsNullOrEmpty(tooltip.TooltipText))
            {
                text.text = tooltip.TooltipText;
                rect.SetAsLastSibling();
                gameObject.SetActive(true);

                // Do after game object is set active
                // Otherwise rebuild will not work
                rect.RebuildLayout();

                // And determine the clamped position
                // after rebuilding to make sure it
                // factors in the rebuilt tooltip size
                if (tooltip is MonoBehaviour monoBehaviour && monoBehaviour.TryGetComponent(out RectTransform rectTransform))
                {
                    Vector3 monoPosition = rectTransform.position;
                    rect.position = monoPosition + (Vector3)offset;
                    rect.ClampTransform(canvasRect, padding);
                }
            }
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