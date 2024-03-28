using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ShapeUtility
    {
        public static Vector3[] GetConePoints(Vector3 position, Vector3 direction, Vector3 axis, float width, float min, float max)
        {
            float delta = width / 2f;
            Quaternion left = Quaternion.Euler(axis * -delta);
            Quaternion right = Quaternion.Euler(axis * delta);
            Vector3[] points = new Vector3[]
            {
                position + (left * direction).normalized * min,
                position + (left * direction).normalized * max,
                position + (right * direction).normalized * max,
                position + (right * direction).normalized * min
            };
            return points;
        }

        public static Vector2[] GetConePoints2D(Vector2 position, Vector2 direction, float width, float min, float max)
        {
            float delta = width / 2f;
            Quaternion left = Quaternion.Euler(0f, 0f, -delta);
            Quaternion right = Quaternion.Euler(0f, 0f, delta);
            Vector2[] points = new Vector2[]
            {
                position + (Vector2) (left * direction).normalized * min,
                position + (Vector2) (left * direction).normalized * max,
                position + (Vector2) (right * direction).normalized * max,
                position + (Vector2) (right * direction).normalized * min
            };
            return points;
        }
    }
}