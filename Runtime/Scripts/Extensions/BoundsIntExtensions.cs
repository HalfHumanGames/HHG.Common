using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class BoundsIntExtensions
    {
        public static BoundsInt Encapsulated(this BoundsInt bounds, Vector3Int point)
        {
            bounds.SetMinMax(Vector3Int.Min(bounds.min, point), Vector3Int.Max(bounds.max, point));
            return bounds;
        }

        public static BoundsInt Encapsulated(this BoundsInt bounds, BoundsInt other)
        {
            bounds.SetMinMax(Vector3Int.Min(bounds.min, other.min), Vector3Int.Max(bounds.max, other.max));
            return bounds;
        }
    }
}