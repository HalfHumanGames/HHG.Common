using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ColorExtensions
    {
        public static Color WithR(this Color color, float r)
        {
            color.r = r;
            return color;
        }

        public static Color WithG(this Color color, float g)
        {
            color.g = g;
            return color;
        }

        public static Color WithB(this Color color, float b)
        {
            color.b = b;
            return color;
        }


        public static Color WithA(this Color color, float a)
        {
            color.a = a;
            return color;
        }

        public static Color WithH(this Color color, float h)
        {
            Color.RGBToHSV(color, out _, out float s, out float v);
            return Color.HSVToRGB(h, s, v);
        }

        public static Color WithS(this Color color, float s)
        {
            Color.RGBToHSV(color, out float h, out _, out float v);
            return Color.HSVToRGB(h, s, v);
        }

        public static Color WithV(this Color color, float v)
        {
            Color.RGBToHSV(color, out float h, out float s, out _);
            return Color.HSVToRGB(h, s, v);
        }

        public static Color WithR32(this Color color, int r)
        {
            color.r = r / 255f;
            return color;
        }

        public static Color WithG32(this Color color, int g)
        {
            color.g = g / 255f;
            return color;
        }

        public static Color WithB32(this Color color, int b)
        {
            color.b = b / 255f;
            return color;
        }


        public static Color WithA32(this Color color, int a)
        {
            color.a = a / 255f;
            return color;
        }

        public static Color Mixed(this Color color, Color other, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                color = (color + other) / 2f;
            }

            return color;
        }

        public static Color Lightened(this Color color, int amount = 1)
        {
            return color.Mixed(Color.white, amount);
        }

        public static Color Darkened(this Color color, int amount = 1)
        {
            return color.Mixed(Color.black, amount);
        }

        public static Color Inverted(this Color color)
        {
            return new Color(1 - color.r, 1f - color.g, 1f - color.b);
        }

        public static void Invert(this Color[] colors)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = colors[i].Inverted();
            }
        }

        public static Color[] Inverted(this Color[] colors)
        {
            Color[] newColors = new Color[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                newColors[i] = colors[i].Inverted();
            }
            return newColors;
        }

        public static bool IsValid(this Color color)
        {
            return color.r >= 0 && color.g >= 0 && color.b >= 0 && color.a >= 0;
        }

        public static float GetH(this Color color)
        {
            Color.RGBToHSV(color, out float h, out _, out _);
            return h;
        }

        public static float GetS(this Color color)
        {
            Color.RGBToHSV(color, out _, out float s, out _);
            return s;
        }

        public static float GetV(this Color color)
        {
            Color.RGBToHSV(color, out _, out _, out float v);
            return v;
        }

        public static string ToHtmlStringRGB(this Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }

        public static string ToHtmlStringRGBA(this Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGBA(color)}";
        }
    }
}