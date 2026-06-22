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

        private void Awake()
        {
            outlineRenderer = GetComponent<Renderer>();
            outlineGraphic = GetComponent<Graphic>();

            if (!outlineMaterial)
            {
                if (outlineRenderer) outlineMaterial = outlineRenderer.material;
                else if (outlineGraphic) outlineMaterial = outlineGraphic.material;
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

            outlineMaterial.SetColor(outlineColor, color);
            outlineMaterial.SetFloat(outlineWidth, width);
        }

        public void Hide()
        {
            outlineMaterial.SetColor(outlineColor, Color.clear);
            outlineMaterial.SetFloat(outlineWidth, 0f);
        }

        private void OnDestroy()
        {
            if (outlineRenderer && outlineMaterial) Destroy(outlineMaterial);
        }
    }
}