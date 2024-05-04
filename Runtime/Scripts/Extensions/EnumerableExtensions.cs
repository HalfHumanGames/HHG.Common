using System.Collections.Generic;
using System;
using System.Linq;

namespace HHG.Common.Runtime
{
    public static class EnumerableExtensions
    {
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

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            Random random = new Random();
            return enumerable.OrderBy(x => random.Next());
        }

        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Shuffle().FirstOrDefault();
        }

        public static IEnumerable<T> GetRandom<T>(this IEnumerable<T> enumerable, int take)
        {
            return enumerable.Shuffle().Take(take);
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Where(item => item != null);
        }
    } 
}