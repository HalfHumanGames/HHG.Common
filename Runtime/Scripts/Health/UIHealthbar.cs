using UnityEngine;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIHealthbar : MonoBehaviour
    {
        [SerializeField] private Transform fill;
        [SerializeField] private Transform background;

        private IReadOnlyHealth source;
        private CanvasGroup canvasGroup;
        private float scale;

        private void Start()
        {
            source = GetComponentInParent<IReadOnlyHealth>(true);
            canvasGroup = GetComponent<CanvasGroup>();
            scale = fill.transform.localScale.x;
        }

        private void Update()
        {
            float perc = source.HealthPerc;
            float scaleX = perc * scale;
            bool isVisible = perc < 1f;
            canvasGroup.alpha = isVisible ? 1f : 0f;
            fill.transform.localScale = fill.transform.localScale.WithX(scaleX);
        }
    }
}
