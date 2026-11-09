using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class SyncSpriteColor : MonoBehaviour
    {
        [SerializeField] private GameObject target;

        private SpriteRenderer spriteRenderer;
        private Image image;
        private LineRenderer lineRenderer;

        private SpriteRenderer targetSpriteRenderer;
        private Image targetImage;
        private LineRenderer targetLineRenderer;

        private void Awake()
        {
            TryGetComponent(out spriteRenderer);
            TryGetComponent(out image);
            TryGetComponent(out lineRenderer);

            target.TryGetComponent(out targetSpriteRenderer);
            target.TryGetComponent(out targetImage);
            target.TryGetComponent(out targetLineRenderer);
        }

        private void LateUpdate()
        {
            Color color = GetTargetColor();

            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }

            if (image != null)
            {
                image.color = color;
            }

            if (lineRenderer != null)
            {
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;
            }
        }

        private Color GetTargetColor()
        {
            if (targetSpriteRenderer != null)
            {
                return targetSpriteRenderer.color;
            }

            if (targetImage != null)
            {
                // Selectable color-tint transitions (e.g. Button states) animate the CanvasRenderer
                // directly instead of the Image's color field, so both must be factored in
                return targetImage.color * targetImage.canvasRenderer.GetColor();
            }

            if (targetLineRenderer != null)
            {
                return targetLineRenderer.startColor;
            }

            return Color.white;
        }
    }
}
