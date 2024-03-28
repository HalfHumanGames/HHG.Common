using UnityEngine;

namespace HHG.Common
{
    public static class CameraExtensions
    {
        public static Bounds GetBounds(this Camera camera)
        {
            return new Bounds
            {
                min = camera.ViewportToWorldPoint(new Vector3(0f, 0f, float.MinValue)),
                max = camera.ViewportToWorldPoint(new Vector3(1f, 1f, float.MaxValue))
            };
        }
    }
}