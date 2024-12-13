using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;

namespace HHG.Common.Runtime
{
    public static class RectTransformExtensions
    {
        public static void ResetAnchoredPos(this RectTransform rect)
        {
            rect.anchoredPosition = Vector2.zero;
        }

        public static void SetAnchoredPos(this RectTransform rect, Vector2 vec2)
        {
            rect.anchoredPosition = vec2;
        }

        public static void SetAnchoredPos(this RectTransform rect, float x, float y)
        {
            rect.anchoredPosition = new Vector2(x, y);
        }

        public static void SetAnchoredPosX(this RectTransform rect, float x)
        {
            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
        }

        public static void SetAnchoredPosY(this RectTransform rect, float y)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y);
        }

        public static void SetSizeDelta(this RectTransform rect, Vector2 vec2)
        {
            rect.sizeDelta = vec2;
        }

        public static void SetSizeDelta(this RectTransform rect, float x, float y)
        {
            rect.sizeDelta = new Vector2(x, y);
        }

        public static void SetSizeDeltaX(this RectTransform rect, float x)
        {
            rect.sizeDelta = new Vector2(x, rect.sizeDelta.y);
        }

        public static void SetSizeDeltaY(this RectTransform rect, float y)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, y);
        }

        public static void SetOffsetMaxX(this RectTransform rect, float maxX)
        {
            rect.offsetMax = rect.offsetMax.WithX(maxX);
        }

        public static void SetOffsetMaxY(this RectTransform rect, float maxY)
        {
            rect.offsetMax = rect.offsetMax.WithY(maxY);
        }

        public static void SetOffsetMinX(this RectTransform rect, float minX)
        {
            rect.offsetMin = rect.offsetMin.WithX(minX);
        }

        public static void SetOffsetMinY(this RectTransform rect, float minY)
        {
            rect.offsetMin = rect.offsetMin.WithY(minY);
        }

        public static void SetOffsetMax(this RectTransform rect, Vector2 offsetMax)
        {
            rect.offsetMax = offsetMax;
        }

        public static void SetOffsetMin(this RectTransform rect, Vector2 offsetMin)
        {
            rect.offsetMin = offsetMin;
        }

        public static void SetOffset(this RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        public static void SetOffset(this RectTransform rect, Vector2 offset)
        {
            rect.offsetMin = offset;
            rect.offsetMax = offset;
        }

        public static void ClampTransform(this RectTransform rect, RectTransform canvas, float padding = 0f)
        {
            Vector2 sizeDelta = rect.sizeDelta * rect.transform.localScale;
            Vector2 anchorOffset = canvas.sizeDelta * (rect.anchorMin - Vector2.one / 2);

            Vector2 maxPivotOffset = sizeDelta * (rect.pivot - Vector2.one / 2 * 2);
            Vector2 minPivotOffset = sizeDelta * (Vector2.one / 2 * 2 - rect.pivot);

            Vector2 position = rect.anchoredPosition;

            float minX = canvas.sizeDelta.x * -0.5f - anchorOffset.x - minPivotOffset.x + sizeDelta.x;
            float maxX = canvas.sizeDelta.x * 0.5f - anchorOffset.x + maxPivotOffset.x;
            float minY = canvas.sizeDelta.y * -0.5f - anchorOffset.y - minPivotOffset.y + sizeDelta.y;
            float maxY = canvas.sizeDelta.y * 0.5f - anchorOffset.y + maxPivotOffset.y;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);

            rect.anchoredPosition = position;
        }

        public static void RebuildLayout(this RectTransform rect)
        {
            LayoutGroup[] layouts = rect.GetComponentsInChildren<LayoutGroup>(true);

            foreach (LayoutGroup layout in layouts)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}