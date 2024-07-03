using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ResolutionExtensions
    {
        public static Vector2Int ToVector2Int(this Resolution resolution)
        {
            return new Vector2Int(resolution.width, resolution.height);
        }

        public static string ToStringWithoutRefreshRate(this Resolution resolution)
        {
            return $"{resolution.width} x {resolution.height}";
        }
    }
}