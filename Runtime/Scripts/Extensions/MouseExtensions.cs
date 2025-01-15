using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public static class MouseExtensions
    {
        public static bool HasInteraction(this Mouse mouse)
        {
            return mouse.delta.ReadValue() != Vector2.zero ||
                   mouse.leftButton.isPressed ||
                   mouse.rightButton.isPressed ||
                   mouse.middleButton.isPressed ||
                   mouse.forwardButton.isPressed ||
                   mouse.backButton.isPressed;
        }

        public static Vector3 GetWorldPosition3D(this Mouse mouse)
        {
            return Camera.main.ScreenToWorldPoint(ScreenUtil.ClampToScreen(mouse.position.value));
        }

        public static Vector2 GetWorldPosition2D(this Mouse mouse)
        {
            return Camera.main.ScreenToWorldPoint(ScreenUtil.ClampToScreen(mouse.position.value));
        }

        public static void WarpCursorPositionToWorld(this Mouse mouse, Vector3 position)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(position);
            Mouse.current.WarpCursorPosition(screenPoint);
        }

        public static void WarpCursorPositionClamped(this Mouse mouse, Vector2 position)
        {
            mouse.WarpCursorPosition(ScreenUtil.ClampToScreen(position));
        }

        public static void WarpCursorPositionToWorldClamped(this Mouse mouse, Vector3 position)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(position);
            mouse.WarpCursorPosition(ScreenUtil.ClampToScreen(screenPoint));
        }

        public static void ClampToScreen(this Mouse mouse)
        {
            mouse.WarpCursorPosition(ScreenUtil.ClampToScreen(mouse.position.value));
        }
    }
}