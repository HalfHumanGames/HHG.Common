using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ResolutionUtil
    {
        private static Resolution[] _resolutions;
        public static Resolution[] Resolutions
        {
            get
            {
                if (_resolutions == null)
                {
                    _resolutions = Screen.resolutions.Distinct(ResolutionSizeEqualityComparer.Instance).OrderBy(r => r.width).ThenBy(r => r.height).ToArray();
                }

                return _resolutions;
            }
        }

        private static string[] _resolutionsFormatted;
        public static string[] ResolutionsFormatted
        {
            get
            {
                if (_resolutionsFormatted == null)
                {
                    _resolutionsFormatted = Resolutions.Select(r => r.ToStringWithoutRefreshRate()).ToArray();
                }

                return _resolutionsFormatted;
            }
        }

        public static Resolution GetMaxResolution()
        {
            int last = Resolutions.Length - 1;
            return Resolutions[last];
        }

        public static int GetResolutionIndex(int width, int  height)
        {
            int len = Resolutions.Length;
            for (int i = 0; i < len; i++)
            {
                Resolution resolution = Resolutions[i];
                if (resolution.width == width && resolution.height == height)
                {
                    return i;
                }
            }
            return len - 1;
        }

        public static void SetResolutionIndex(int index, out int width, out int height)
        {
            int len = Resolutions.Length;
            index = Mathf.Clamp(index, 0, len - 1);
            Resolution resolution = Resolutions[index];
            width = resolution.width;
            height = resolution.height;
        }

        public static IEnumerable<string> GetResolutionsFormatted()
        {
            return Resolutions.Select(r => r.ToStringWithoutRefreshRate());
        }
    }
}