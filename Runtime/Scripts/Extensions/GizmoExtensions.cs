using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class GizmoExtensions
    {
        public static void DrawCone(Vector3 position, Vector3 direction, Vector3 axis, float width, float min, float max)
        {
            Vector3[] points = ShapeUtility.GetConePoints(position, direction, axis, width, min, max);
            Gizmos.DrawLineStrip(points, true);
        }

        public static void DrawCone2D(Vector3 position, Vector3 direction, float width, float min, float max)
        {
            Vector3[] points = ShapeUtility.GetConePoints(position, direction, Vector3.forward, width, min, max);
            Gizmos.DrawLineStrip(points, true);
        }
    }
}