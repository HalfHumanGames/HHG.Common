using UnityEngine;
using UnityEngine.UI;

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
            Vector2 position = rect.localPosition;
            Vector3[] rectCorners = new Vector3[4];
            Vector3[] canvasCorners = new Vector3[4];
            rect.GetWorldCorners(rectCorners);
            canvas.GetWorldCorners(canvasCorners);

            // Clamp right/left
            if (rectCorners[2].x > canvasCorners[2].x)
            {
                position.x = (canvas.rect.width * .5f) - (rect.rect.width * (1f - rect.pivot.x)) + padding;
            }
            else if (rectCorners[0].x < canvasCorners[0].x)
            {
                position.x = (-canvas.rect.width * .5f) + (rect.rect.width * rect.pivot.x) + padding;
            }

            // Clamp top/bottom
            if (rectCorners[2].y > canvasCorners[2].y)
            {
                position.y = (canvas.rect.height * .5f) - (rect.rect.height * (1f - rect.pivot.y)) + padding;
            }
            else if (rectCorners[0].y < canvasCorners[0].y)
            {
                position.y = (-canvas.rect.height * .5f) + (rect.rect.height * rect.pivot.y) + padding;
            }

            rect.localPosition = position;
        }

        public static void RebuildLayout(this RectTransform rect)
        {
            LayoutGroup[] layouts = rect.GetComponentsInChildren<LayoutGroup>(true);

            foreach (LayoutGroup layout in layouts)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rect.GetComponent<RectTransform>());
        }
    }
}