using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace HHG.Common.Runtime
{
    public static class EventSystemExtensions
    {
        public static InputSystemUIInputModule GetInputSystemUIInputModule(this EventSystem eventSystem)
        {
            return eventSystem.currentInputModule as InputSystemUIInputModule;
        }
    }
}