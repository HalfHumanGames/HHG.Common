using UnityEngine;

namespace HHG.Common.Runtime
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

        public static void SetLayerCulling(this Camera camera, string layerName, bool cull)
        {
            int layer = LayerMask.NameToLayer(layerName);

            if (cull)
            {
                camera.cullingMask |= 1 << layer;
            }
            else
            {
                camera.cullingMask &= ~(1 << layer);
            }
        }
    }
}