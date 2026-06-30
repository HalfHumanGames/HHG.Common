#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace Dungeonspire
{
    public class SpriteNormalMapper : MonoBehaviour
    {
        private enum TargetMode { Self, Renderer, Children, TargetChildren, Renderers }

        [SerializeField] private bool mapInAwake;
        [SerializeField, Min(0f)] private float normalStrength = 1f;
        [SerializeField] private string normalMapProperty = "_NormalMap";
        [SerializeField] private string normalStrengthProperty = "_NormalStrength";
        [SerializeField] private string nameFormat = "{0} - Normal Map (generated)";

        [Header("Generation")]
        [SerializeField, Min(0f)] private float normalMultiplier = 5f;
        [SerializeField, Min(0f)] private int normalSmoothness = 0;

        [Header("Target")]
        [SerializeField] private TargetMode targetMode = TargetMode.Children;
        [SerializeField, ShowIf(nameof(targetMode), TargetMode.Renderer)]       private SpriteRenderer targetRenderer;
        [SerializeField, ShowIf(nameof(targetMode), TargetMode.TargetChildren)] private Transform targetChildren;
        [SerializeField, ShowIf(nameof(targetMode), TargetMode.Renderers)]      private List<SpriteRenderer> targetRenderers = new List<SpriteRenderer>();

        private int normalMapPropertyId;
        private int normalStrengthPropertyId;
        private List<Material> materials = new List<Material>();
        private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        private void Awake()
        {
            normalMapPropertyId = Shader.PropertyToID(normalMapProperty);
            normalStrengthPropertyId = Shader.PropertyToID(normalStrengthProperty);

            switch (targetMode)
            {
                case TargetMode.Self:
                    SpriteRenderer self = GetComponent<SpriteRenderer>();
                    if (self != null) spriteRenderers.Add(self);
                    break;
                case TargetMode.Renderer:
                    if (targetRenderer != null) spriteRenderers.Add(targetRenderer);
                    break;
                case TargetMode.Children:
                    GetComponentsInChildren(spriteRenderers);
                    break;
                case TargetMode.TargetChildren:
                    if (targetChildren != null) targetChildren.GetComponentsInChildren(spriteRenderers);
                    break;
                case TargetMode.Renderers:
                    spriteRenderers.AddRange(targetRenderers);
                    break;
            }

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                materials.Add(spriteRenderer.material);
            }

            if (mapInAwake) Map();
        }

        public void Map()
        {
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                SpriteRenderer spriteRenderer = spriteRenderers[i];

                if (spriteRenderer.sprite == null)
                {
                    continue;
                }

                string textureName = spriteRenderer.sprite.texture.name;
                string generatedName = string.Format(nameFormat, textureName);
                Texture2D normalMap = Resources.Load<Texture2D>("Normals/" + generatedName);

#if UNITY_EDITOR
                if (normalMap == null)
                {
                    normalMap = GenerateAndSaveNormalMap(spriteRenderer.sprite.texture, generatedName);
                }
#endif

                if (normalMap == null)
                {
                    continue;
                }

                spriteRenderer.material.SetTexture(normalMapPropertyId, normalMap);
                spriteRenderer.material.SetFloat(normalStrengthPropertyId, normalStrength);
            }
        }

        private void OnDestroy()
        {
            foreach (Material material in materials)
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                Map();
            }
        }

#if UNITY_EDITOR
        private Texture2D GenerateAndSaveNormalMap(Texture2D sourceTexture, string generatedName)
        {
            string folder = Application.dataPath + "/Resources/Normals";
            string path = "Assets/Resources/Normals/" + generatedName + ".png";

            Directory.CreateDirectory(folder);
            byte[] bytes = CreateNormalMap(sourceTexture, normalMultiplier, normalSmoothness).EncodeToPNG();
            File.WriteAllBytes(folder + "/" + generatedName + ".png", bytes);
            AssetDatabase.ImportAsset(path);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.NormalMap;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
#endif

        public Texture2D CreateNormalMap(Texture2D t, float normalMultiplier = 5f, int normalSmooth = 0)
        {
            int width = t.width;
            int height = t.height;
            Color[] sourcePixels = t.GetPixels();
            Color[] resultPixels = new Color[width * height];
            Vector3 vScale = new Vector3(0.3333f, 0.3333f, 0.3333f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = x + y * width;
                    Vector3 cSampleNegXNegY = GetPixelClamped(sourcePixels, x - 1, y - 1, width, height);
                    Vector3 cSampleZerXNegY = GetPixelClamped(sourcePixels, x, y - 1, width, height);
                    Vector3 cSamplePosXNegY = GetPixelClamped(sourcePixels, x + 1, y - 1, width, height);
                    Vector3 cSampleNegXZerY = GetPixelClamped(sourcePixels, x - 1, y, width, height);
                    Vector3 cSamplePosXZerY = GetPixelClamped(sourcePixels, x + 1, y, width, height);
                    Vector3 cSampleNegXPosY = GetPixelClamped(sourcePixels, x - 1, y + 1, width, height);
                    Vector3 cSampleZerXPosY = GetPixelClamped(sourcePixels, x, y + 1, width, height);
                    Vector3 cSamplePosXPosY = GetPixelClamped(sourcePixels, x + 1, y + 1, width, height);

                    float fSampleNegXNegY = Vector3.Dot(cSampleNegXNegY, vScale);
                    float fSampleZerXNegY = Vector3.Dot(cSampleZerXNegY, vScale);
                    float fSamplePosXNegY = Vector3.Dot(cSamplePosXNegY, vScale);
                    float fSampleNegXZerY = Vector3.Dot(cSampleNegXZerY, vScale);
                    float fSamplePosXZerY = Vector3.Dot(cSamplePosXZerY, vScale);
                    float fSampleNegXPosY = Vector3.Dot(cSampleNegXPosY, vScale);
                    float fSampleZerXPosY = Vector3.Dot(cSampleZerXPosY, vScale);
                    float fSamplePosXPosY = Vector3.Dot(cSamplePosXPosY, vScale);

                    float edgeX = (fSampleNegXNegY - fSamplePosXNegY) * 0.25f + (fSampleNegXZerY - fSamplePosXZerY) * 0.5f + (fSampleNegXPosY - fSamplePosXPosY) * 0.25f;
                    float edgeY = (fSampleNegXNegY - fSampleNegXPosY) * 0.25f + (fSampleZerXNegY - fSampleZerXPosY) * 0.5f + (fSamplePosXNegY - fSamplePosXPosY) * 0.25f;

                    Vector2 vEdge = new Vector2(edgeX, edgeY) * normalMultiplier;
                    Vector3 norm = new Vector3(vEdge.x, vEdge.y, 1.0f).normalized;
                    resultPixels[index] = new Color(norm.x * 0.5f + 0.5f, norm.y * 0.5f + 0.5f, norm.z * 0.5f + 0.5f, 1);
                }
            }

            if (normalSmooth > 0)
            {
                resultPixels = SmoothNormals(resultPixels, width, height, normalSmooth);
            }

            Texture2D texNormal = new Texture2D(width, height, TextureFormat.RGB24, false, false);
            texNormal.SetPixels(resultPixels);
            texNormal.Apply();
            return texNormal;
        }

        private static Vector3 GetPixelClamped(Color[] pixels, int x, int y, int width, int height)
        {
            x = Mathf.Clamp(x, 0, width - 1);
            y = Mathf.Clamp(y, 0, height - 1);
            Color c = pixels[x + y * width];
            return new Vector3(c.r, c.g, c.b);
        }

        private static Color[] SmoothNormals(Color[] pixels, int width, int height, int normalSmooth)
        {
            Color[] smoothedPixels = new Color[pixels.Length];
            float step = 0.00390625f * normalSmooth;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float pixelsToAverage = 0.0f;
                    Color c = pixels[x + y * width];
                    pixelsToAverage++;

                    for (int offsetY = -normalSmooth; offsetY <= normalSmooth; offsetY++)
                    {
                        for (int offsetX = -normalSmooth; offsetX <= normalSmooth; offsetX++)
                        {
                            if (offsetX == 0 && offsetY == 0) continue;

                            int sampleX = Mathf.Clamp(x + offsetX, 0, width - 1);
                            int sampleY = Mathf.Clamp(y + offsetY, 0, height - 1);

                            c += pixels[sampleX + sampleY * width];
                            pixelsToAverage++;
                        }
                    }

                    smoothedPixels[x + y * width] = c / pixelsToAverage;
                }
            }

            return smoothedPixels;
        }
    }
}
