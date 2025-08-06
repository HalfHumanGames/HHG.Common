using System.Collections.Generic;

namespace HHG.Common.Runtime
{
    public static class HashSetExtensions
    {
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                hashSet.Add(item);
            }
        }

        public static void RemoveRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                hashSet.Remove(item);
            }
        }
    }
}