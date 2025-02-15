using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ListExtensions
    {
        public static void Remove<T>(this IList<T> list, System.Func<T, bool> func)
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

        public static void RemoveAll<T>(this IList<T> list, System.Func<T, bool> func)
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

        // Shuffle affects the list itself while Shuffled
        // returns a new IEnumerable, but does not affect
        // the source list itself
        public static void Shuffle<T>(this IList<T> list)
        {
            int count = list.Count;
            int last = count - 1;
            for (int i = 0; i < last; ++i)
            {
                int rand = Random.Range(i, count);
                T temp = list[i];
                list[i] = list[rand];
                list[rand] = temp;
            }
        }

        public static void SortedInsert<T>(this List<T> list, T item, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            int index = list.BinarySearch(item, comparer);

            if (index < 0)
            {
                index = ~index;
            }

            list.Insert(index, item);
        }

        public static void ResortItem<T>(this List<T> list, T item, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;
            list.Remove(item);
            list.SortedInsert(item, comparer);
        }

        public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            int i = 0;
            foreach (T element in list)
            {
                if (Equals(element, item))
                {
                    return i;
                }
                    
                i++;
            }
            return -1;
        }

        public static int FindIndex<T>(this IReadOnlyList<T> list, System.Func<T, bool> check)
        {
            int i = 0;
            foreach (T element in list)
            {
                if (check(element))
                {
                    return i;
                }

                i++;
            }
            return -1;
        }

        public static bool Resize<T>(this List<T> list, int size, T template, Transform parent = null) where T : MonoBehaviour
        {
            bool adjusted = list.Count != size;

            while (list.Count < size)
            {
                T item = Object.Instantiate(template, parent);
                item.gameObject.SetActive(true);
                list.Add(item);
            }

            while (list.Count > size)
            {
                T item = list[list.Count - 1];
                Object.Destroy(item.gameObject);
                list.RemoveAt(list.Count - 1);
            }

            return adjusted;
        }
    }
}