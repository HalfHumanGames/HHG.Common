using System.Collections.Generic;

namespace HHG.Common.Runtime
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            foreach(T item in items)
            {
                source.Add(item);
            }
        }
    }
}