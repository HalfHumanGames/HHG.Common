using UnityEngine;

namespace HHG.Common.Runtime
{
    public class ResolutionUtil
    {
        public static Resolution GetMaxResolution()
        {
            int count = Screen.resolutions.Length;
            return Screen.resolutions[count - 1];
        }

        public static int GetResolutionIndex(int width, int  height)
        {
            int len = Screen.resolutions.Length;
            for (int i = 0; i < len; i++)
            {
                Resolution resolution = Screen.resolutions[i];
                if (resolution.width == width && resolution.height == height)
                {
                    return i;
                }
            }
            return len - 1;
        }

        public static void SetResolutionIndex(int index, out int width, out int height)
        {
            int len = Screen.resolutions.Length;
            index = Mathf.Clamp(index, 0, len - 1);
            Resolution resolution = Screen.resolutions[index];
            width = resolution.width;
            height = resolution.height;
        }
    }
}