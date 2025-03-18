using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class CanvasExtensions
    {
        public static Vector2 WorldToAnchoredPoint(this Canvas canvas, RectTransform containerRectTransform, Vector3 worldPoint)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);

            Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRectTransform, screenPoint, camera, out Vector2 localPoint))
            {
                return localPoint;
            }

            return Vector2.zero;
        }
    }
}