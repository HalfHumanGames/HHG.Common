using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class SpriteOutline : MonoBehaviour
    {
        private static readonly int outlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int outlineWidth = Shader.PropertyToID("_OutlineWidth");

        [SerializeField] private Color color = Color.white;
        [SerializeField] private float width = .008f;

        // Serialize in case instantiated while hidden and outlineGraphic.material is null
        // This ensures the created instance retains a reference to the outline material
        [SerializeField, HideInInspector] private Material outlineMaterial;

        private Renderer outlineRenderer;
        private Graphic outlineGraphic;
        private MaterialPropertyBlock block;

        private void Awake()
        {
            outlineRenderer = GetComponent<Renderer>();
            outlineGraphic = GetComponent<Graphic>();

            if (outlineRenderer)
            {
                block = new MaterialPropertyBlock();
                outlineRenderer.GetPropertyBlock(block);
            }

            if (outlineGraphic && !outlineMaterial)
            {
                outlineMaterial = outlineGraphic.material;
            }

            Hide();
        }

        private void OnEnable()
        {
            Show();
        }

        private void OnDisable()
        {
            Hide();
        }

        public void Show()
        {
            Show(color, width);
        }

        public void Show(Color newColor, float newWidth)
        {
            color = newColor;
            width = newWidth;
            
            if (outlineGraphic)
            {
                outlineGraphic.material = outlineMaterial;
            }

            if (outlineRenderer)
            {
                block.SetColor(outlineColor, color);
                block.SetFloat(outlineWidth, width);
                outlineRenderer.SetPropertyBlock(block);
            }

            if (outlineGraphic)
            {
                outlineGraphic.material.SetColor(outlineColor, color);
                outlineGraphic.material.SetFloat(outlineWidth, width);
            }
        }

        public void Hide()
        {
            if (outlineRenderer)
            {
                block.SetColor(outlineColor, Color.clear);
                block.SetFloat(outlineWidth, 0f);
                outlineRenderer.SetPropertyBlock(block);
            }
            
            if (outlineGraphic)
            {
                outlineGraphic.material = null;
            }
        }
    }
}