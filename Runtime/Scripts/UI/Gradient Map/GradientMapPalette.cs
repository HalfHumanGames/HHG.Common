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
        [SerializeField] private List<GradientMapRamp> ramps = new List<GradientMapRamp>();

        private Texture2D texture;
        private Material material;

        public float GetRowCoordinate(int row)
        {
            int count = Mathf.Max(1, ramps.Count);
            return (Mathf.Clamp(row, 0, count - 1) + .5f) / count;
        }

        private void Build()
        {
            int rows = Mathf.Max(1, ramps.Count);

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
                Gradient gradient = y < ramps.Count ? ramps[y].Gradient : null;

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
            // Only rebuild what already exists so validating an unused palette stays free
            if (material != null || texture != null)
            {
                Build();
            }

            Changed?.Invoke();
        }

        private void OnDisable()
        {
            this.SmartDestroy(material);
            this.SmartDestroy(texture);
        }
    }
}
