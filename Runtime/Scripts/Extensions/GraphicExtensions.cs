using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public static class GraphicExtensions
    {
        // Mirrors Renderer.material: the first call instances the shared material and later calls return that same instance
        public static Material GetMaterialInstance(this Graphic graphic)
        {
            Material material = graphic.material;

            // Materials created at runtime have negative instance IDs, so this one is already an instance
            if (material.GetInstanceID() < 0)
            {
                return material;
            }

            Material instance = new Material(material);
            graphic.material = instance;
            return instance;
        }
    }
}
