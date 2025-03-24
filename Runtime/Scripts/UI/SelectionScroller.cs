using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(ScrollRect))]
    public class SelectionScroller : MonoBehaviour
    {
        private ScrollRect scrollRect;
        private GameObject previousSelection;

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        private void Update()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;

            if (selected == null)
            {
                return;
            }

            if (selected == previousSelection)
            {
                return;
            }

            if (!selected.transform.IsChildOf(scrollRect.content))
            {
                return;
            }

            GameObject current = selected;

            while (current.transform.parent != scrollRect.content)
            {
                current = current.transform.parent.gameObject;
            }


            RectTransform selectedRectTransform = current.GetComponent<RectTransform>();
            RectTransform viewportRectTransform = scrollRect.viewport;

            Vector3[] selectedWorldCorners = new Vector3[4];
            Vector3[] viewportWorldCorners = new Vector3[4];

            selectedRectTransform.GetWorldCorners(selectedWorldCorners);
            viewportRectTransform.GetWorldCorners(viewportWorldCorners);

            int siblingIndex = current.transform.GetSiblingIndex();

            bool isAbove = selectedWorldCorners[1].y > viewportWorldCorners[1].y;
            bool isBelow = selectedWorldCorners[0].y < viewportWorldCorners[0].y;
            bool isNotVisible = isAbove || isBelow;
            bool isFirst = siblingIndex == 0;
            bool isLast = siblingIndex == current.transform.parent.childCount - 1;

            if (isNotVisible || isFirst || isLast)
            {
                float xPos = scrollRect.content.anchoredPosition.x;
                float yPos = 0f;

                if (isFirst)
                {
                    yPos = 0f;
                }
                else if (isLast)
                {
                    yPos = scrollRect.content.rect.height - viewportRectTransform.rect.height;
                }
                else
                {
                    yPos = isAbove ?
                        -selectedRectTransform.localPosition.y - (selectedRectTransform.rect.height / 2) :
                        -selectedRectTransform.localPosition.y + (selectedRectTransform.rect.height / 2) - viewportRectTransform.rect.height;
                }

                scrollRect.content.anchoredPosition = new Vector2(xPos, yPos);
            }

            previousSelection = selected;
        }
    }
}