using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public static class EventSystemExtensions
    {
        private const int uiLayer = 5;
        private static int frame;
        private static bool isDirty { get => frame != Time.frameCount; set => frame = value ? Time.frameCount - 1 : Time.frameCount; }
        private static List<RaycastResult> _results = new List<RaycastResult>();
        private static IReadOnlyList<RaycastResult> results {
            get
            {
                if (isDirty)
                {
                    isDirty = false;
                    PointerEventData eventData = new PointerEventData(EventSystem.current);
                    eventData.position = Mouse.current.position.value;
                    EventSystem.current.RaycastAll(eventData, _results);
                }
                return _results;
            }
        }

        public static InputSystemUIInputModule GetInputSystemUIInputModule(this EventSystem eventSystem)
        {
            // Use GetComponent as a fallback since currentInputModule is not in available in Start or Awake (becomes available in Update)
            return eventSystem.currentInputModule as InputSystemUIInputModule ?? eventSystem.GetComponent<InputSystemUIInputModule>();
        }

        public static bool IsPointerOverUI(this EventSystem eventSystem, bool isEventProcess = false)
        {
            if (isEventProcess || eventSystem.IsPointerOverGameObject())
            {
                return results.Count > 0 && results[0].gameObject.layer == uiLayer;
            }
            return false;
        }
        
        public static bool IsPointerOverGameObject(this EventSystem eventSystem, GameObject gameObject, bool topmost = false, bool isEventProcess = false)
        {
            if (isEventProcess || eventSystem.IsPointerOverGameObject())
            {
                return topmost ? results[0].gameObject == gameObject : results.Any(r => r.gameObject == gameObject);
            }
            return false;
        }

        public static Selectable GetCurrentSelectable(this EventSystem eventSystem)
        {
            GameObject selected = eventSystem.currentSelectedGameObject;
            return selected != null ? selected.GetComponent<Selectable>() : null;
        }

        public static bool TryGetCurrentSelection(this EventSystem eventSystem, out Selectable selection)
        {
            selection = null;
            GameObject selected = eventSystem.currentSelectedGameObject;
            return selected && selected.TryGetComponent(out selection);
        }

        public static void ReselectSelectedGameObject(this EventSystem eventSystem)
        {
            GameObject selected = eventSystem.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selected);
        }
    }
}