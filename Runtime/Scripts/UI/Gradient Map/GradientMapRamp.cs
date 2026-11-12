using UnityEngine;

namespace HHG.Common.Runtime
{
    // A sub-asset of a GradientMapPalette, so graphics can reference a ramp directly instead of by row
    public class GradientMapRamp : ScriptableObject
    {
        public GradientMapPalette Palette { get => palette; internal set => palette = value; }
        public Gradient Gradient => gradient;

        [SerializeField, HideInInspector] private GradientMapPalette palette;
        [SerializeField] private Gradient gradient = new Gradient();

        private void OnValidate()
        {
            if (palette != null)
            {
                palette.Rebuild();
            }
        }
    }
}
