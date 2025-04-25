using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public class LinkListener : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler
    {
        public UnityEvent<string> LinkClicked => linkClicked;
        public UnityEvent<string> LinkEntered => linkEntered;
        public UnityEvent<string> LinkExited => linkExited;

        [SerializeField] private UnityEvent<string> linkClicked;
        [SerializeField] private UnityEvent<string> linkEntered;
        [SerializeField] private UnityEvent<string> linkExited;

        private TMP_Text label;
        private string current;

        private void Awake()
        {
            label = GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (TryFindIntersectingLink(out string id))
            {
                linkClicked?.Invoke(id);
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (TryFindIntersectingLink(out string id))
            {
                if (current == id)
                {
                    return;
                }

                if (current != null)
                {
                    linkExited?.Invoke(current);
                }

                current = id;
                linkEntered?.Invoke(current);
            }
            else if (current != null)
            {
                linkExited?.Invoke(current);
                current = null;
            }
        }

        private bool TryFindIntersectingLink(out string id)
        {
            Vector2 mousePosition = Mouse.current?.position.ReadValue() ?? Vector2.zero;

            int linkIndex = label != null
                ? TMP_TextUtilities.FindIntersectingLink(label, mousePosition, null)
                : -1;

            id = linkIndex != -1
                ? label.textInfo.linkInfo[linkIndex].GetLinkID()
                : null;

            return id != null;
        }
    }
}
