using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class MathUtil
    {
        public static float ManhattanDistance(Vector2 point1, Vector2 point2)
        {
            return Mathf.Abs(point1.x - point2.x) + Mathf.Abs(point1.y - point2.y);
        }
    }
}