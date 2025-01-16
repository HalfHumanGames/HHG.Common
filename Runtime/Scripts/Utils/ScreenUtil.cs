using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ScreenUtil
    {
        public static Vector2 GetScreenCenter()
        {
            return new Vector2(Screen.width / 2f, Screen.height / 2f);
        }

        public static Vector2 ClampToScreen(Vector2 position)
        {  
            position.x = Mathf.Clamp(position.x, 1f, Screen.currentResolution.width - 1f);
            position.y = Mathf.Clamp(position.y, 1f, Screen.currentResolution.height - 1f);
            return position;
        }

        public static bool IsInsideScreen(Vector2 position)
        {
            return position.x >= 1f && position.x <= Screen.currentResolution.width - 1f &&
                   position.y >= 1f && position.y <= Screen.currentResolution.height - 1f;
        }
    }
}