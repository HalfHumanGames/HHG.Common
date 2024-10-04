using System.Threading;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class TextureUtil
    {
        public class ThreadData
        {
            public int Start;
            public int End;

            public ThreadData(int s, int e)
            {
                Start = s;
                End = e;
            }
        }

        private static Color[] texColors;
        private static Color[] newColors;
        private static int w;
        private static float ratioX;
        private static float ratioY;
        private static int w2;
        private static int finishCount;
        private static Mutex mutex;

        public static void Scale(Texture2D tex, int newWidth, int newHeight, TextureScaleMode scaleMode)
        {
            bool useBilinear = scaleMode == TextureScaleMode.Bilinear;
            texColors = tex.GetPixels();
            newColors = new Color[newWidth * newHeight];

            if (useBilinear)
            {
                ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
                ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
            }
            else
            {
                ratioX = ((float)tex.width) / newWidth;
                ratioY = ((float)tex.height) / newHeight;
            }

            w = tex.width;
            w2 = newWidth;
            int cores = Mathf.Min(SystemInfo.processorCount, newHeight);
            int slice = newHeight / cores;
            finishCount = 0;

            if (mutex == null)
            {
                mutex = new Mutex(false);
            }

            if (cores > 1)
            {
                int i = 0;
                ThreadData threadData;

                for (i = 0; i < cores - 1; i++)
                {
                    threadData = new ThreadData(slice * i, slice * (i + 1));
                    ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
                    Thread thread = new Thread(ts);
                    thread.Start(threadData);
                }

                threadData = new ThreadData(slice * i, newHeight);

                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }

                while (finishCount < cores)
                {
                    Thread.Sleep(1);
                }
            }
            else
            {
                ThreadData threadData = new ThreadData(0, newHeight);

                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }
            }

            tex.Reinitialize(newWidth, newHeight, tex.format, tex.mipmapCount > 0);
            tex.SetPixels(newColors);
            tex.Apply();

            texColors = null;
            newColors = null;
        }

        private static void BilinearScale(object obj)
        {
            ThreadData threadData = (ThreadData)obj;

            for (int y = threadData.Start; y < threadData.End; y++)
            {
                int yFloor = (int)Mathf.Floor(y * ratioY);
                int y1 = yFloor * w;
                int y2 = (yFloor + 1) * w;
                int yw = y * w2;

                for (int x = 0; x < w2; x++)
                {
                    int xFloor = (int)Mathf.Floor(x * ratioX);
                    float xLerp = x * ratioX - xFloor;
                    newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
                                                           ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
                                                           y * ratioY - yFloor);
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        private static void PointScale(object obj)
        {
            ThreadData threadData = (ThreadData)obj;

            for (int y = threadData.Start; y < threadData.End; y++)
            {
                int thisY = (int)(ratioY * y) * w;
                int yw = y * w2;
                for (int x = 0; x < w2; x++)
                {
                    newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
        {
            return new Color(c1.r + (c2.r - c1.r) * value,
                             c1.g + (c2.g - c1.g) * value,
                             c1.b + (c2.b - c1.b) * value,
                             c1.a + (c2.a - c1.a) * value);
        }

        public static void Rotate(Texture2D tex, float angle)
        {
            Color[] pixels = tex.GetPixels();

            int w = tex.width;
            int h = tex.height;

            Color[] rotatedPixels = new Color[w * h];

            // Center of the texture
            float centerX = w / 2.0f;
            float centerY = h / 2.0f;

            // Precompute rotation matrix values
            float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
            float sin = Mathf.Sin(angle * Mathf.Deg2Rad);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    // Translate to origin (center of the texture)
                    float u = x - centerX;
                    float v = y - centerY;

                    // Rotate around the origin
                    float rotatedX = u * cos - v * sin;
                    float rotatedY = u * sin + v * cos;

                    // Translate back to the texture space
                    rotatedX += centerX;
                    rotatedY += centerY;

                    rotatedPixels[x + y * w] = GetPixel(pixels, rotatedX, rotatedY, w, h);
                }
            }

            tex.SetPixels(rotatedPixels);
            tex.Apply();
        }

        private static Color GetPixel(Color[] pixels, float x, float y, int width, int height)
        {
            int x1 = Mathf.FloorToInt(x);
            int y1 = Mathf.FloorToInt(y);

            if (x1 >= width || x1 < 0 || y1 >= height || y1 < 0)
            {
                return Color.clear; // Transparent if out of bounds
            }

            return pixels[x1 + (y1 * width)];
        }

    }

}