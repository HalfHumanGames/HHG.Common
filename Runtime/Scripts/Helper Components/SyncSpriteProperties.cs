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
        private Material materialInstance;
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
            materialInstance.SetColor(colorPropertyId, GetSpriteColor());
            materialInstance.SetVector(flipPropertyId, GetFlipVector());
        }

        private void EnsureMaterialIsCached()
        {
            if (materialInstance == null)
            {
                if (spriteRenderer == null && TryGetComponent(out spriteRenderer))
                {
                    type = Type.SpriteRenderer;
                    materialInstance = spriteRenderer.material;
                }
                else if (image == null && TryGetComponent(out image))
                {
                    type = Type.Image;
                    materialInstance = image.GetMaterialInstance();
                }
                else if (lineRenderer == null && TryGetComponent(out lineRenderer))
                {
                    type = Type.LineRenderer;
                    materialInstance = lineRenderer.material;
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
            Vector3 lossyScale = transform.lossyScale;
            float xSign = Mathf.Sign(lossyScale.x);
            float ySign = Mathf.Sign(lossyScale.y);

            // Sprite renderer flips mirror the sprite on top of the transform's scale
            if (type == Type.SpriteRenderer)
            {
                if (spriteRenderer.flipX)
                {
                    xSign = -xSign;
                }

                if (spriteRenderer.flipY)
                {
                    ySign = -ySign;
                }
            }

            float scaleSign = Mathf.Sign(xSign * ySign * lossyScale.z);
            return new Vector4(xSign, ySign, scaleSign, ySign);
        }

        private void OnDestroy()
        {
            // Every type uses its own material instance, so it's always ours to destroy
            if (materialInstance)
            {
                Destroy(materialInstance);
            }
        }
    }
}
