using UnityEngine;

namespace HHG.Common.Runtime
{
    [ExecuteInEditMode]
    public class ColorInjector : MonoBehaviour
    {
        private readonly int colorProperty = Shader.PropertyToID("_Color");

        [SerializeField] private Mode mode;

        private SpriteRenderer spriteRenderer;
        private Material spriteMaterial;

        public enum Mode
        {
            OnEnable,
            LateUpdate,
            Manual
        }

        private void OnEnable()
        {
            if (mode == Mode.OnEnable) InjectColor();
        }

        private void LateUpdate()
        {
            if (mode == Mode.LateUpdate) InjectColor();
        }

        public void InjectColor()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteMaterial == null) spriteMaterial = spriteRenderer.material;

            spriteMaterial.SetColor(colorProperty, spriteRenderer.color);
        }

        private void OnDestroy()
        {
            if (spriteMaterial) Destroy(spriteMaterial);
        }
    }
}
