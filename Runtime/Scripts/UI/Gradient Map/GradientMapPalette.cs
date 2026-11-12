using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [CreateAssetMenu(fileName = "Gradient Map Palette", menuName = "HHG/UI/Gradient Map Palette")]
    public class GradientMapPalette : ScriptableObject
    {
        private static readonly int rampTexId = Shader.PropertyToID("_RampTex");

        public event System.Action Changed;

        public Material Material
        {
            get
            {
                if (material == null || texture == null)
                {
                    Build();
                }

                return material;
            }
        }

        [SerializeField] private Shader shader;
        [SerializeField, Min(2)] private int resolution = 256;
        [SerializeField] private SubAssetList<GradientMapRamp> ramps = new SubAssetList<GradientMapRamp>();

        private Texture2D texture;
        private Material material;

        public float GetRowCoordinate(GradientMapRamp ramp)
        {
            IReadOnlyList<GradientMapRamp> items = ramps.Items;
            int row = 0;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == ramp)
                {
                    row = i;
                    break;
                }
            }

            return (row + .5f) / Mathf.Max(1, items.Count);
        }

        internal void Rebuild()
        {
            // Only rebuild what already exists so validating an unused palette stays free
            if (material != null || texture != null)
            {
                Build();
            }

            Changed?.Invoke();
        }

        private void Build()
        {
            IReadOnlyList<GradientMapRamp> items = ramps.Items;
            int rows = Mathf.Max(1, items.Count);

            if (texture == null || texture.width != resolution || texture.height != rows)
            {
                this.SmartDestroy(texture);

                // sRGB because gradient colors are authored in gamma space
                texture = new Texture2D(resolution, rows, TextureFormat.RGBA32, false, false)
                {
                    name = $"{name} Ramps",
                    hideFlags = HideFlags.DontSave,
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear
                };
            }

            Color32[] pixels = new Color32[resolution * rows];

            for (int y = 0; y < rows; y++)
            {
                Gradient gradient = y < items.Count && items[y] != null ? items[y].Gradient : null;

                for (int x = 0; x < resolution; x++)
                {
                    pixels[y * resolution + x] = gradient != null ? gradient.Evaluate(x / (resolution - 1f)) : Color.white;
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false);

            if (material == null)
            {
                material = new Material(shader)
                {
                    name = $"{name} Material",
                    hideFlags = HideFlags.DontSave
                };
            }

            material.SetTexture(rampTexId, texture);
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            // Link ramps added through the Sub-Asset tools back to this palette
            foreach (GradientMapRamp ramp in ramps.Items)
            {
                if (ramp != null && ramp.Palette != this)
                {
                    ramp.Palette = this;
                    UnityEditor.EditorUtility.SetDirty(ramp);
                }
            }
#endif
            Rebuild();
        }

        private void OnDisable()
        {
            this.SmartDestroy(material);
            this.SmartDestroy(texture);
        }
    }
}
