using UnityEngine;

namespace HHG.Common.Runtime
{
    public class SyncSpriteProperties : MonoBehaviour
    {
        [SerializeField] private Mode mode;
        [SerializeField] private string colorProperty = "_Color";
        [SerializeField] private string flipProperty = "_SpriteFlip";

        private SpriteRenderer spriteRenderer;
        private Material spriteMaterial;
        private int colorPropertyId;
        private int flipPropertyId;

        public enum Mode
        {
            OnEnable,
            LateUpdate,
            Manual
        }

        private void Awake()
        {
            colorPropertyId = Shader.PropertyToID(colorProperty);
            flipPropertyId = Shader.PropertyToID(flipProperty);
        }

        private void OnEnable()
        {
            if (mode == Mode.OnEnable) SyncProperties();
        }

        private void LateUpdate()
        {
            if (mode == Mode.LateUpdate) SyncProperties();
        }

        public void SyncProperties()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteMaterial == null) spriteMaterial = spriteRenderer.material;

            spriteMaterial.SetColor(colorPropertyId, spriteRenderer.color);
            spriteMaterial.SetVector(flipPropertyId, GetFlipVector());
        }

        private Vector4 GetFlipVector()
        {
            float flipX = spriteRenderer.flipX ? -1f : 1f;
            float flipY = spriteRenderer.flipY ? -1f : 1f;
            Vector3 lossyScale = spriteRenderer.transform.lossyScale;
            float scaleSign = Mathf.Sign(lossyScale.x * lossyScale.y * lossyScale.z * flipX * flipY);
            return new Vector4(flipX, flipY, scaleSign, flipY);
        }

        private void OnDestroy()
        {
            if (spriteMaterial) Destroy(spriteMaterial);
        }
    }
}
