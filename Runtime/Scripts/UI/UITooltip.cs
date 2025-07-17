using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UITooltip : MonoBehaviour
    {
        public ITooltip Current => current;

        [SerializeField, FormerlySerializedAs("text")] private TextMeshProUGUI label;
        [SerializeField] private LayoutElement labelLayoutElement;
        [SerializeField] private float maxWidth = 300;
        [SerializeField] private Vector2 defaultOffset = new Vector2(32, -32);

        private RectTransform rect;
        private RectTransform canvasRect;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private ITooltip current;
        private string tooltipText;
        private PositionMode positionMode;
        private OffsetMode offsetMode;
        private Vector2 customOffset;

        public enum PositionMode
        {
            FollowMouse,
            FixedPosition
        }

        public enum OffsetMode
        {
            Default,
            Custom,
            None,
        }

        private void Awake()
        {
            Locator.Register(this);
            rect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            canvasRect = canvas.GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!string.IsNullOrEmpty(tooltipText))
            {
                // Only follow mouse if the cursor is visible
                if (positionMode == PositionMode.FollowMouse && Cursor.visible && Mouse.current != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Mouse.current.position.value, canvas.worldCamera, out Vector2 point))
                {
                    Vector3 position = canvasRect.TransformPoint(point);
                    Reposition(position);
                }
            }
        }

        public void Show(ITooltip tooltip, PositionMode positionMode = PositionMode.FollowMouse, OffsetMode offsetMode = OffsetMode.Default, Vector2 customOffset = default)
        {
            Initialize(tooltip, positionMode, offsetMode, customOffset);
            ShowInternal(current.TooltipText, current.TooltipPosition);
        }

        public void Show(string text, Vector3 position, PositionMode positionMode = PositionMode.FollowMouse, OffsetMode offsetMode = OffsetMode.Default, Vector2 customOffset = default)
        {
            Initialize(null, positionMode, offsetMode, customOffset);
            ShowInternal(text, position);
        }

        public void Hide()
        {
            tooltipText = null;
            gameObject.SetActive(false);
        }

        private void Initialize(ITooltip tooltip, PositionMode newPositionMode = PositionMode.FollowMouse, OffsetMode newOffsetMode = OffsetMode.Default, Vector2 newCustomOffset = default)
        {
            current = tooltip;
            positionMode = newPositionMode;
            offsetMode = newOffsetMode;
            customOffset = newCustomOffset;
        }

        private void ShowInternal(string text, Vector3 position)
        {
            tooltipText = text;

            // Don't show if null or empty tooltip text
            if (!string.IsNullOrEmpty(tooltipText))
            {
                label.text = tooltipText;
                gameObject.SetActive(true);

                // Do after game object is set active
                // Otherwise rebuild will not work
                Resize();
                Reposition(position);
            }
        }

        [ContextMenu(nameof(Resize))]
        private void Resize()
        {
            labelLayoutElement.preferredWidth = -1;
            rect.RebuildLayout();

            if (rect.sizeDelta.x > maxWidth)
            {
                labelLayoutElement.preferredWidth = maxWidth;
                rect.RebuildLayout();
            }
        }

        private void Reposition(Vector3 position)
        {
            rect.SetAsLastSibling();
            rect.position = position;
            rect.anchoredPosition += GetOffset();
            rect.ClampTransform(canvasRect);
        }

        private Vector2 GetOffset()
        {
            return offsetMode switch
            {
                OffsetMode.Default => defaultOffset,
                OffsetMode.Custom => customOffset,
                _ => Vector2.zero
            };
        }

        private void OnDestroy()
        {
            Locator.Unregister(this);
        }
    }
}