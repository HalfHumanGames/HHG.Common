using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public class FollowMouse : MonoBehaviour
    {
        public Vector2 Offset;

        private void Update()
        {
            transform.position = Mouse.current.GetWorldPosition2D() + Offset;
        }
    }
}