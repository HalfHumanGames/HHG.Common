using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public static class GamepadExtensions
    {
        public static bool HasInteraction(this Gamepad gamepad)
        {
            return Mathf.Abs(gamepad.leftStick.x.ReadValue()) > 0.1f ||
                   Mathf.Abs(gamepad.leftStick.y.ReadValue()) > 0.1f ||
                   Mathf.Abs(gamepad.rightStick.x.ReadValue()) > 0.1f ||
                   Mathf.Abs(gamepad.rightStick.y.ReadValue()) > 0.1f ||
                   gamepad.buttonSouth.isPressed ||
                   gamepad.buttonWest.isPressed ||
                   gamepad.buttonNorth.isPressed ||
                   gamepad.buttonEast.isPressed ||
                   gamepad.leftTrigger.isPressed ||
                   gamepad.rightTrigger.isPressed ||
                   gamepad.leftShoulder.isPressed ||
                   gamepad.rightShoulder.isPressed;
        }
    }
}