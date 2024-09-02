using UnityEngine;

namespace HHG.Common.Runtime
{
    // This script works with both sprites and images
    public class UIHealthbar : MonoBehaviour
    {
        [SerializeField] private Transform fill;
        [SerializeField] private Transform background;

        private IReadOnlyHealth source;
        private CanvasGroup canvasGroup;
        private RendererGroup rendererGroup;
        private float scale;

        private void Start()
        {
            source = GetComponentInParent<IReadOnlyHealth>(true);
            canvasGroup = GetComponent<CanvasGroup>();
            rendererGroup = GetComponent<RendererGroup>();
            scale = fill.transform.localScale.x;
        }

        private void Update()
        {
            float perc = source.HealthPerc;
            float scaleX = perc * scale;
            bool isVisible = perc < 1f;

            if (canvasGroup)
            {
                canvasGroup.alpha = isVisible ? 1f : 0f;
            }

            if (rendererGroup)
            {
                rendererGroup.enabled = isVisible;
            }

            fill.transform.localScale = fill.transform.localScale.WithX(scaleX);
        }
    }
}
