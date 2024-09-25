using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HHG.Common.Runtime
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> func)
        {
            foreach (T item in source)
            {
                func(item);
            }
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            if (!source.Any())
            {
                return default;
            }

            return source.Aggregate((min, current) => Comparer<TKey>.Default.Compare(selector(current), selector(min)) < 0 ? current : min);
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

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

        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Shuffled().FirstOrDefault();
        }

        public static IEnumerable<T> GetRandom<T>(this IEnumerable<T> enumerable, int take)
        {
            return enumerable.Shuffled().Take(take);
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Where(item => item != null);
        }

        public static T SelectByWeight<T>(this IEnumerable<T> source, Func<T, int> getter)
        {
            Func<T, int> min = item => Mathf.Max(getter(item), 1);
            int rand = Random.Range(0, source.Sum(min));
            return source.FirstOrDefault(item => (rand -= min(item)) < 0);
        }
    } 
}