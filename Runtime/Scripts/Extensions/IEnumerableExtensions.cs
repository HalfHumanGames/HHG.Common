using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class IEnumerableExtensions
    {
        private static System.Random random = new System.Random();

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
        public static IEnumerable<T> Shuffled<T>(this IEnumerable<T> enumerable, System.Random rand = null)
        {
            rand ??= random;
            return enumerable.OrderBy(x => rand.Next(0, int.MaxValue));
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

        // Weights <= 0 are never picked, unless nothing has weight; then every item is equally likely
        public static T SelectByWeight<T>(this IList<T> source, System.Func<T, int> getter, System.Random rand = null)
        {
            int index = WeightedIndex(source, getter, rand ?? random);
            return index < 0 ? default : source[index];
        }

        public static T SelectByWeight<T>(this IEnumerable<T> source, System.Func<T, int> getter, System.Random rand = null)
        {
            // Copied once so a lazy source isn't enumerated twice
            List<T> items = Pool.GetList<T>();
            items.AddRange(source);
            T item = items.SelectByWeight(getter, rand);
            Pool.ReleaseList(items);
            return item;
        }

        // Picks up to amount distinct items, each pick weighted over what's left
        public static IEnumerable<T> SelectByWeight<T>(this IEnumerable<T> source, int amount, System.Func<T, int> getter, System.Random rand = null)
        {
            rand ??= random;
            List<T> remaining = Pool.GetList<T>();
            remaining.AddRange(source);

            // Returns the list to the pool even if the caller stops enumerating early
            try
            {
                for (int i = 0; i < amount && remaining.Count > 0; i++)
                {
                    T item = remaining[WeightedIndex(remaining, getter, rand)];

                    for (int j = remaining.Count - 1; j >= 0; j--)
                    {
                        if (EqualityComparer<T>.Default.Equals(remaining[j], item))
                        {
                            remaining.RemoveAt(j);
                        }
                    }

                    yield return item;
                }
            }
            finally
            {
                Pool.ReleaseList(remaining);
            }
        }

        // Returns -1 when items is empty
        private static int WeightedIndex<T>(IList<T> items, System.Func<T, int> getter, System.Random rand)
        {
            int total = 0;

            for (int i = 0; i < items.Count; i++)
            {
                total += Mathf.Max(getter(items[i]), 0);
            }

            if (total <= 0)
            {
                int uniformIndex = rand.Next(0, items.Count);
                return items.Count > 0 ? uniformIndex : -1;
            }

            int roll = rand.Next(0, total);

            for (int i = 0; i < items.Count; i++)
            {
                roll -= Mathf.Max(getter(items[i]), 0);

                if (roll < 0)
                {
                    return i;
                }
            }

            return -1;
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