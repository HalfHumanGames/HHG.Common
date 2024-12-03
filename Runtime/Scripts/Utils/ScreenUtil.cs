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
            position.x = Mathf.Clamp(position.x, 0f, Screen.currentResolution.width);
            position.y = Mathf.Clamp(position.y, 0f, Screen.currentResolution.height);
            return position;
        }
    }
}