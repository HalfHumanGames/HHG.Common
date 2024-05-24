using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace HHG.Common.Runtime
{
    public static class EventSystemExtensions
    {
        public static InputSystemUIInputModule GetInputSystemUIInputModule(this EventSystem eventSystem)
        {
            // Use GetComponent as a fallback since currentInputModule is not in available in Start or Awake (becomes available in Update)
            return eventSystem.currentInputModule as InputSystemUIInputModule ?? eventSystem.GetComponent<InputSystemUIInputModule>();
        }
    }
}