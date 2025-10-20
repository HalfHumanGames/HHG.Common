using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T item)
        {
            return source.Where(x => !EqualityComparer<T>.Default.Equals(x, item));
        }

        public static void ForEach<T>(this IEnumerable<T> source, System.Action<T> func)
        {
            foreach (T item in source)
            {
                func(item);
            }
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, System.Func<TSource, TKey> selector)
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (selector == null) throw new System.ArgumentNullException(nameof(selector));

            if (!source.Any())
            {
                return default;
            }

            return source.Aggregate((min, current) => Comparer<TKey>.Default.Compare(selector(current), selector(min)) < 0 ? current : min);
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, System.Func<TSource, TKey> selector)
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (selector == null) throw new System.ArgumentNullException(nameof(selector));

            if (!source.Any())
            {
                return default;
            }

            return source.Aggregate((max, current) => Comparer<TKey>.Default.Compare(selector(current), selector(max)) > 0 ? current : max);
        }

        // Shuffle affects the list itself while Shuffled
        // returns a new IEnumerable, but does not affect
        // the source list itself
        public static IEnumerable<T> Shuffled<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.OrderBy(x => Random.Range(0, int.MaxValue));
        }

        public static T RandomOrDefault<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Shuffled().FirstOrDefault();
        }

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> enumerable, int take)
        {
            return enumerable.Shuffled().Take(take);
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Where(item => item != null);
        }

        public static T SelectByWeight<T>(this IEnumerable<T> source, System.Func<T, int> getter)
        {
            System.Func<T, int> min = item => Mathf.Max(getter(item), 1);
            int rand = Random.Range(0, source.Sum(min));
            return source.FirstOrDefault(item => (rand -= min(item)) < 0);
        }

        public static IEnumerable<T> TakeByWeight<T>(this IEnumerable<T> source, int amount, System.Func<T, int> getter)
        {
            System.Func<T, int> min = item => Mathf.Max(getter(item), 1);
            HashSet<T> taken = new HashSet<T>();
            int sum = source.Sum(min);

            for (int i = 0; i < amount; i++)
            {
                int rand = Random.Range(0, sum);

                foreach (T item in source)
                {
                    if (taken.Contains(item))
                    {
                        continue;
                    }

                    if ((rand -= min(item)) < 0)
                    {
                        yield return item;
                        taken.Add(item);
                        sum -= min(item);
                        break;
                    }
                }
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T search)
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (search == null) throw new System.ArgumentNullException(nameof(search));

            int index = 0;
            foreach (T element in source)
            {
                if (EqualityComparer<T>.Default.Equals(element, search))
                {
                    return index;
                }
                index++;
            }

            return -1;
        }

        public static int FindIndex<T>(this IEnumerable<T> source, System.Func<T, bool> search)
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (search == null) throw new System.ArgumentNullException(nameof(search));

            int index = 0;
            foreach (T element in source)
            {
                if (search(element))
                {
                    return index;
                }
                index++;
            }

            return -1;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            return !source.Any();
        }
    } 
}