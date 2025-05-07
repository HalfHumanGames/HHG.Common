using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class SpriteExtensions
    {
        public static Color[,] GetPixels(this Sprite sprite)
        {
            Texture2D texture = sprite.texture;
            Rect rect = sprite.textureRect;
            int width = (int)rect.width;
            int height = (int)rect.height;

            Color[] pixels1D = texture.GetPixels((int)rect.x, (int)rect.y, width, height);
            Color[,] pixels2D = new Color[width, height];

            SpritePackingRotation rotation = sprite.packed ? sprite.packingRotation : SpritePackingRotation.None;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    int tx = x, ty = y;

                    if (rotation == SpritePackingRotation.FlipHorizontal)
                    {
                        tx = width - 1 - x;
                    }
                    else if (rotation == SpritePackingRotation.FlipVertical)
                    {
                        ty = height - 1 - y;
                    }
                    else if (rotation == SpritePackingRotation.Rotate180)
                    {
                        tx = width - 1 - x;
                        ty = height - 1 - y;
                    }

                    pixels2D[tx, ty] = pixels1D[index];
                }
            }

            return pixels2D;
        }
    }
}