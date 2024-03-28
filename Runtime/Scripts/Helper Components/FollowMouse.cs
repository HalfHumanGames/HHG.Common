using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common
{
    public class FollowMouse : MonoBehaviour
    {
        public Vector2 Offset;

        void Update()
        {
            transform.position = Mouse.current.GetWorldPosition2D() + (Vector3)Offset;
        }
    }

}