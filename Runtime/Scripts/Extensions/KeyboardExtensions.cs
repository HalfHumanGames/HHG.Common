using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public static class KeyboardExtensions
    {
        public static bool HasInteraction(this Keyboard keyboard)
        {
            return keyboard.anyKey.isPressed;
        }
    }
}