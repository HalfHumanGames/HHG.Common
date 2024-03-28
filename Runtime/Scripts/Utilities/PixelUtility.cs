using System.Linq;
using UnityEngine;

public static class PixelUtility
{
    public static Color[] Noise(int width, int height, float scale, int seed) => Noise(width, height, scale, scale, seed);
    public static Color[] Noise(int width, int height, float scaleX, float scaleY, int seed)
    {
        Color[] pixels = new Color[width * height];

        float y = 0.0f;

        while (y < height)
        {
            float x = 0.0F;
            while (x < width)
            {
                float xCoord = x / width * scaleX + seed;
                float yCoord = y / height * scaleY + seed;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                int index = (int)y * width + (int)x;
                pixels[index] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        return pixels;
    }

    public static Color[] SmudgeVertical(Color[] pixels, int width, int height, int smudgeSize)
    {
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                float darkness = 1 - pixels[index].grayscale;
                int smudge = (int)(darkness * smudgeSize);

                if (smudge == 0)
                {
                    continue;
                }

                var range = smudge < 0 ? Enumerable.Range(smudge, 1) : Enumerable.Range(1, smudge);

                foreach (int offset in range)
                {
                    int smudgeIndex = (y + offset) * width + x;

                    if (smudgeIndex >= 0 && smudgeIndex < pixels.Length)
                    {
                        pixels[smudgeIndex] = Color.Lerp(pixels[index], pixels[smudgeIndex], .5f);
                    }
                }
            }
        }
        return pixels;
    }

    public static Color[] Contrast(Color[] pixels, float contrastFactor)
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            float grayscale = pixels[i].grayscale;
            float factor = (contrastFactor + 1f) / 2f;
            float adjustedValue = ((grayscale - 0.5f) * factor) + 0.5f;

            pixels[i] = new Color(adjustedValue, adjustedValue, adjustedValue, pixels[i].a);
        }

        return pixels;
    }

    public static Color[] Shade(Color[] pixels, Color[] heightmap, float minBrightness, float maxBrightness)
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            float brightness = Mathf.Lerp(minBrightness, maxBrightness, 1 - heightmap[i].r);
            pixels[i] = pixels[i] * brightness;
        }
        return pixels;
    }

    public static Color[] Blur(Color[] pixels, int width, int height, int blurSize)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color sum = Color.black;

                for (int offsetY = -blurSize; offsetY <= blurSize; offsetY++)
                {
                    for (int offsetX = -blurSize; offsetX <= blurSize; offsetX++)
                    {
                        int sampleX = Mathf.Clamp(x + offsetX, 0, width - 1);
                        int sampleY = Mathf.Clamp(y + offsetY, 0, height - 1);
                        sum += pixels[sampleY * width + sampleX];
                    }
                }

                pixels[y * width + x] = sum / ((blurSize * 2 + 1) * (blurSize * 2 + 1));
            }
        }

        return pixels;
    }
}