using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class AutoScroller : MonoBehaviour
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

        RectTransform selectedRectTransform = selected.GetComponent<RectTransform>();
        RectTransform viewportRectTransform = scrollRect.viewport;

        Vector3[] selectedWorldCorners = new Vector3[4];
        Vector3[] viewportWorldCorners = new Vector3[4];

        selectedRectTransform.GetWorldCorners(selectedWorldCorners);
        viewportRectTransform.GetWorldCorners(viewportWorldCorners);

        bool isAbove = selectedWorldCorners[1].y > viewportWorldCorners[1].y;
        bool isBelow = selectedWorldCorners[0].y < viewportWorldCorners[0].y;
        bool isNotVisible = isAbove || isBelow;

        if (isNotVisible)
        {
            // NOTE: localPosition might not work if the selected game object is not
            // an immediate child of the scroll rect's content game object.
            float xPos = scrollRect.content.anchoredPosition.x;
            float yPos = isAbove ?
                -selectedRectTransform.localPosition.y - (selectedRectTransform.rect.height / 2) :
                -selectedRectTransform.localPosition.y + (selectedRectTransform.rect.height / 2) - viewportRectTransform.rect.height;
            scrollRect.content.anchoredPosition = new Vector2(xPos, yPos);
        }

        previousSelection = selected;
    }
}