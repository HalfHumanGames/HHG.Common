using UnityEngine;

namespace HHG.Common.Runtime
{
    [ExecuteInEditMode]
    public class ColorInjector : MonoBehaviour
    {
        private readonly int colorProperty = Shader.PropertyToID("_Color");

        [SerializeField] private Mode mode;

        private SpriteRenderer spriteRenderer;
        private MaterialPropertyBlock propertyBlock;

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
            propertyBlock ??= new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(colorProperty, spriteRenderer.color);
            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
