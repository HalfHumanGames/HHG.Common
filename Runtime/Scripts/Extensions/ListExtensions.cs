using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ListExtensions
    {
        public static void Remove<T>(this IList<T> list, Func<T, bool> func)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (func(list[i]))
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        public static void RemoveAll<T>(this IList<T> list, Func<T, bool> func)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (func(list[i]))
                {
                    list.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void RemoveRange<T>(this IList<T> list, IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                list.Remove(item);
            }
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static void Resize<T>(this IList<T> list, int size)
        {
            size = Mathf.Max(size, 0);

            if (size == 0)
            {
                list.Clear();
            }
            
            while (list.Count > size)
            {
                list.RemoveAt(list.Count - 1);
            }
            
            while (list.Count < size)
            {
                list.Add(default);
            }
        }
    }
}