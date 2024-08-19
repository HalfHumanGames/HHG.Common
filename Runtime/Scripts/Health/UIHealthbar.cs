using UnityEngine;

namespace HHG.Common.Runtime
{
    public class UIHealthbar : MonoBehaviour
    {
        [SerializeField] private Transform fill;
        [SerializeField] private Transform background;

        protected IReadOnlyHealth source;

        private float scale;

        private void Start()
        {
            source = GetComponentInParent<IReadOnlyHealth>();
            scale = fill.transform.localScale.x;
        }

        private void Update()
        {
            float perc = source.HealthPerc;
            float scaleX = perc * scale;
            bool isVisible = perc < 1f;
            gameObject.SetActive(isVisible);
            fill.transform.localScale = fill.transform.localScale.WithX(scaleX);
        }
    }
}
