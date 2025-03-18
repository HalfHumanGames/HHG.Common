using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(ScrollRect))]
    public class AutoScroller : MonoBehaviour
    {
        public bool AutoScroll { get => autoScroll; set => autoScroll = value; }
        public float ScrollSpeed { get => scrollSpeed; set => scrollSpeed = value; }

        [SerializeField] private bool autoScroll = true;
        [SerializeField] private float scrollSpeed = 50f;

        private ScrollRect scrollRect;

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        private void Update()
        {
            if (autoScroll)
            {
                float contentHeight = scrollRect.content.rect.height;
                float viewportHeight = scrollRect.viewport.rect.height;

                if (contentHeight <= viewportHeight)
                {
                    return;
                }

                if (scrollRect.verticalNormalizedPosition > 0f)
                {
                    float normalizedSpeed = scrollSpeed / (contentHeight - viewportHeight);
                    scrollRect.verticalNormalizedPosition = Mathf.MoveTowards(scrollRect.verticalNormalizedPosition, 0f, normalizedSpeed * Time.deltaTime);
                }
            }
        }

        public void Restart()
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}
