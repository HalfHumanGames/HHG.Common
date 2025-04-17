using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class SpriteExtensions
    {
        public static Color[,] GetPixels(this Sprite sprite)
        {
            Texture2D texture = sprite.texture;
            Rect rect = sprite.rect;

            int width = Mathf.FloorToInt(rect.width);
            int height = Mathf.FloorToInt(rect.height);

            Color[,] pixels = new Color[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int textureX = Mathf.FloorToInt(rect.x) + x;
                    int textureY = Mathf.FloorToInt(rect.y) + y;
                    pixels[x, y] = texture.GetPixel(textureX, textureY);
                }
            }

            return pixels;
        }
    }
}