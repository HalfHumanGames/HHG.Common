using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public static class MouseExtensions
    {
        public static Vector3 GetWorldPosition(this Mouse mouse)
        {
            return Camera.main.ScreenToWorldPoint(mouse.position.value);
        }

        public static Vector3 GetWorldPosition2D(this Mouse mouse)
        {
            return Camera.main.ScreenToWorldPoint(mouse.position.value).WithZ(0);
        }
    }
}