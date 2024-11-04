using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class RectExtensions
    {
        public static Rect Encapsulated(this Rect rect, Rect other)
        {
            return new Rect(
                Mathf.Min(rect.min.x, other.min.x),
                Mathf.Min(rect.min.y, other.min.y),
                Mathf.Max(rect.max.x, other.max.x),
                Mathf.Max(rect.max.y, other.max.y)
            );
        }
    }
}