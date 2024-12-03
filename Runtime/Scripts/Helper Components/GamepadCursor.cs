using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public class GamepadCursor : MonoBehaviour
    {
        public float CursorSpeed { get => speed; set => speed = value; }

        [SerializeField] private float speed = 1000f;

        private Vector2 position;

        private void OnEnable()
        {
            position = Mouse.current.position.ReadValue();
        }

        private void Update()
        {
            if (Gamepad.current != null)
            {
                Vector2 stick = Gamepad.current.leftStick.ReadValue();

                if (stick != Vector2.zero)
                {
                    position = ScreenUtil.ClampToScreen(position + stick * speed * Time.unscaledDeltaTime);
                    Mouse.current.WarpCursorPosition(position);
                }
                else
                {
                    position = Mouse.current.position.ReadValue();
                }
            }
        }
    }
}
