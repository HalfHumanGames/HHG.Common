using UnityEngine.Pool;

namespace HHG.Common.Runtime
{
    public partial class ObjectPool
    {
        public static PooledObject<T> Get<T>(out T item) where T : class, new()
        {
            return GenericPool<T>.Get(out item);
        }

        public static T Get<T>() where T : class, new()
        {
            return GenericPool<T>.Get();
        }

        public static void Release<T>(T item) where T : class, new()
        {
            GenericPool<T>.Release(item);
        }
    }
}