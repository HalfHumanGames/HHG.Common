using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class RectTransformExtensions
    {
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
    }
}