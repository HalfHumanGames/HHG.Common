using UnityEngine;
using UnityEngine.Pool;

namespace HHG.Common.Runtime
{
    public class GameObjectPool<T> : ObjectPool<T> where T : Object
    {
        //private static readonly IPoolable[] empty = new IPoolable[0];
        //private static readonly Dictionary<T, IPoolable[]> poolablesMap = new Dictionary<T, IPoolable[]>();

        public GameObjectPool(T template, Transform parent = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) : base(() => Create(template, parent), OnGet, OnRelease, Destroy, collectionCheck, defaultCapacity, maxSize)
        {

        }

        public GameObjectPool(GameObject template, Transform parent = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) : this(template.GetComponent<T>(), parent, collectionCheck, defaultCapacity, maxSize)
        {

        }

        private static T Create(T template, Transform parent)
        {
            T item = Object.Instantiate(template, parent);
            //poolablesMap[item] = GetPoolables(item);
            return item;
        }

        private static void OnGet(T item)
        {
            GetGameObject(item).SetActive(true);

            //if (!poolablesMap.TryGetValue(item, out IPoolable[] poolables))
            //{
            //    poolablesMap[item] = poolables = GetPoolables(item);
            //}

            //foreach (IPoolable poolable in poolables)
            //{
            //    poolable.OnGetFromPool();
            //}
        }

        private static void OnRelease(T item)
        {
            GetGameObject(item).SetActive(false);

            //if (!poolablesMap.TryGetValue(item, out IPoolable[] poolables))
            //{
            //    poolablesMap[item] = poolables = GetPoolables(item);
            //}

            //foreach (IPoolable poolable in poolables)
            //{
            //    poolable.OnReleaseToPool();
            //}
        }

        private static void Destroy(T item)
        {
            //poolablesMap.Remove(item);
            Object.Destroy(GetGameObject(item));
        }

        private static GameObject GetGameObject(T item)
        {
            return item switch
            {
                GameObject go => go,
                Component c => c.gameObject,
                _ => null
            };
        }

        //private static IPoolable[] GetPoolables(T item)
        //{
        //    return item switch
        //    {
        //        GameObject go => go.GetComponentsInChildren<IPoolable>(),
        //        Component c => c.GetComponentsInChildren<IPoolable>(),
        //        _ => empty
        //    };
        //}
    }
}
