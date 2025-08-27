using System.Collections.Generic;
using UnityEngine.Pool;

namespace HHG.Common.Runtime
{
    // Wrapper similar to Unity's GenericPool, but with simplified API
    // and conditionCheck set to false to increase performance optimization.
    public static class Pool
    {
        // Objects
        public static PooledObject<T> Get<T>(out T item) where T : class, new() => PoolT<T>.Get(out item);
        public static T Get<T>() where T : class, new() => PoolT<T>.Get();
        public static void Release<T>(T item) where T : class, new() => PoolT<T>.Release(item);

        // Lists
        public static PooledObject<List<T>> GetList<T>(out List<T> item) => CollectionPoolT<List<T>, T>.Get(out item);
        public static List<T> GetList<T>() => CollectionPoolT<List<T>, T>.Get();
        public static void ReleaseList<T>(List<T> collection) => CollectionPoolT<List<T>, T>.Release(collection);

        // Hash Sets
        public static PooledObject<HashSet<T>> GetHashSet<T>(out HashSet<T> item) => CollectionPoolT<HashSet<T>, T>.Get(out item);
        public static HashSet<T> GetHashSet<T>() => CollectionPoolT<HashSet<T>, T>.Get();
        public static void ReleaseHashSet<T>(HashSet<T> collection) => CollectionPoolT<HashSet<T>, T>.Release(collection);

        // Queues
        public static PooledObject<Queue<T>> GetQueue<T>(out Queue<T> item) => QueuePool<T>.Get(out item);
        public static Queue<T> GetQueue<T>() => QueuePool<T>.Get();
        public static void ReleaseQueue<T>(Queue<T> collection) => QueuePool<T>.Release(collection);

        // Stacks
        public static PooledObject<Stack<T>> GetStack<T>(out Stack<T> item) => StackPool<T>.Get(out item);
        public static Stack<T> GetStack<T>() => StackPool<T>.Get();
        public static void ReleaseStack<T>(Stack<T> collection) => StackPool<T>.Release(collection);

        // Dictionaries
        public static PooledObject<Dictionary<K, V>> GetDictionary<K, V>(out Dictionary<K, V> item) => CollectionPoolT<Dictionary<K, V>, KeyValuePair<K, V>>.Get(out item);
        public static Dictionary<K, V> GetDictionary<K, V>() => CollectionPoolT<Dictionary<K, V>, KeyValuePair<K, V>>.Get();
        public static void ReleaseDictionary<K, V>(Dictionary<K, V> collection) => CollectionPoolT<Dictionary<K, V>, KeyValuePair<K, V>>.Release(collection);

        // Internal helpers
        private class PoolT<T> where T : class, new()
        {
            internal static readonly ObjectPool<T> pool = new ObjectPool<T>(() => new T(), collectionCheck: false);

            public static T Get() => pool.Get();
            public static PooledObject<T> Get(out T item) => pool.Get(out item);
            public static void Release(T item) => pool.Release(item);
        }

        private class CollectionPoolT<TCollection, TItem> where TCollection : class, ICollection<TItem>, new()
        {
            internal static readonly ObjectPool<TCollection> pool = new ObjectPool<TCollection>(() => new TCollection(), null, c => c.Clear(), collectionCheck: false);

            public static TCollection Get() => pool.Get();
            public static PooledObject<TCollection> Get(out TCollection value) => pool.Get(out value);
            public static void Release(TCollection toRelease) => pool.Release(toRelease);
        }

        // Needs specific implementation since does not implement ICollection<T>
        private class StackPool<T>
        {
            internal static readonly ObjectPool<Stack<T>> pool = new ObjectPool<Stack<T>>(() => new Stack<T>(), null, c => c.Clear(), collectionCheck: false);

            public static Stack<T> Get() => pool.Get();
            public static PooledObject<Stack<T>> Get(out Stack<T> value) => pool.Get(out value);
            public static void Release(Stack<T> toRelease) => pool.Release(toRelease);
        }

        // Needs specific implementation since does not implement ICollection<T>
        private class QueuePool<T>
        {
            internal static readonly ObjectPool<Queue<T>> pool = new ObjectPool<Queue<T>>(() => new Queue<T>(), null, c => c.Clear(), collectionCheck: false);

            public static Queue<T> Get() => pool.Get();
            public static PooledObject<Queue<T>> Get(out Queue<T> value) => pool.Get(out value);
            public static void Release(Queue<T> toRelease) => pool.Release(toRelease);
        }
    }
}
