using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class SyncSpriteProperties : MonoBehaviour
    {
        [SerializeField] private Mode mode;
        [SerializeField] private string colorProperty = "_Color";
        [SerializeField] private string flipProperty = "_SpriteFlip";

        private SpriteRenderer spriteRenderer;
        private LineRenderer lineRenderer;
        private Image image;
        private Material spriteMaterial;
        private Type type;
        private int colorPropertyId;
        private int flipPropertyId;

        public enum Mode
        {
            OnEnable,
            LateUpdate,
            Manual
        }

        private enum Type
        {
            SpriteRenderer,
            Image,
            LineRenderer
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
            EnsureMaterialIsCached();
            spriteMaterial.SetColor(colorPropertyId, GetSpriteColor());
            spriteMaterial.SetVector(flipPropertyId, GetFlipVector());
        }

        private void EnsureMaterialIsCached()
        {
            if (spriteMaterial == null)
            {
                if (spriteRenderer == null && TryGetComponent(out spriteRenderer))
                {
                    type = Type.SpriteRenderer;
                    spriteMaterial = spriteRenderer.material;
                }
                else if (image == null && TryGetComponent(out image))
                {
                    type = Type.Image;
                    spriteMaterial = image.material;
                }
                else if (lineRenderer == null && TryGetComponent(out lineRenderer))
                {
                    type = Type.LineRenderer;
                    spriteMaterial = lineRenderer.material;
                }
            }
        }

        private Color GetSpriteColor()
        {
            return type switch
            {
                Type.SpriteRenderer => spriteRenderer.color,
                Type.Image => image.color,
                Type.LineRenderer => lineRenderer.startColor,
                _ => Color.white
            };
        }

        private Vector4 GetFlipVector()
        {
            // Then get flip vector from scale
            Vector3 lossyScale = transform.lossyScale;
            float xSign = Mathf.Sign(lossyScale.x);
            float ySign = Mathf.Sign(lossyScale.y);
            float scaleSign = Mathf.Sign(lossyScale.x * lossyScale.y * lossyScale.z);
            return new Vector4(xSign, ySign, scaleSign, ySign);
        }

        private void OnDestroy()
        {
            // Images do NOT create new material instances like sprite renderer's do
            if (spriteMaterial && type == Type.SpriteRenderer) Destroy(spriteMaterial);
        }
    }
}
