using UnityEngine;

namespace HHG.Common.Runtime
{
    public class ColorInjector : MonoBehaviour
    {
        [SerializeField] private Mode mode;
        [SerializeField] private string propertyName = "_Color";

        private SpriteRenderer spriteRenderer;
        private Material spriteMaterial;
        private int propertyId;

        public enum Mode
        {
            OnEnable,
            LateUpdate,
            Manual
        }

        private void Awake()
        {
            propertyId = Shader.PropertyToID(propertyName);
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

            spriteMaterial.SetColor(propertyId, spriteRenderer.color);
        }

        private void OnDestroy()
        {
            if (spriteMaterial) Destroy(spriteMaterial);
        }
    }
}
