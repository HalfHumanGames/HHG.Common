using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ArrayExtensions
    {
        public static T GetRandom<T>(this T[] array)
        {
            return array.Length == 0 ? default : array[Random.Range(0, array.Length)];
        }

        public static T[] RotatedClockwise<T>(this T[] array, int rotation = 1)
        {
            int size = (int) Mathf.Sqrt(array.Length);
            T[] source = new T[array.Length];
            T[] destination = new T[array.Length];

            System.Array.Copy(array, source, array.Length);
            System.Array.Copy(array, destination, array.Length);

            for (int interation = 0; interation < rotation; interation++)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        destination[j * size + (size - 1 - i)] = source[i * size + j];
                    }
                }
                System.Array.Copy(destination, source, array.Length);
            }

            return destination;
        }

        public static T[] RotatedCounterclockwise<T>(this T[] array, int rows, int cols, int rotation = 1)
        {
            T[] source = new T[array.Length];
            T[] destination = new T[array.Length];

            System.Array.Copy(array, source, rows * cols);
            System.Array.Copy(array, destination, rows * cols);

            for (int interation = 0; interation < rotation; interation++)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        destination[(cols - 1 - j) * rows + i] = source[i * cols + j];
                    }
                }
                System.Array.Copy(destination, source, rows * cols);
            }

            return destination;
        }

        public static void ForEach<T>(this T[] array, System.Action<int, T> action)
        {
            if (array == null)
            {
                throw new System.ArgumentNullException(nameof(array));
            }

            if (action == null)
            {
                throw new System.ArgumentNullException(nameof(action));
            }

            for (int i = 0; i < array.Length; i++)
            {
                action(i, array[i]);
            }
        }

        public static void ForEach<T>(this T[,] array, System.Action<int, int, T> action)
        {
            if (array == null)
            {
                throw new System.ArgumentNullException(nameof(array));
            }

            if (action == null)
            {
                throw new System.ArgumentNullException(nameof(action));
            }

            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    action(i, j, array[i, j]);
                }
            }
        }
    }
}