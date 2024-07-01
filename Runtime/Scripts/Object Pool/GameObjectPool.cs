using UnityEngine;
using UnityEngine.Pool;

namespace HHG.Common.Runtime
{
    public class GameObjectPool : ObjectPool<GameObject>
    {
        public GameObjectPool(GameObject template, Transform parent = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) : base(() => Create(template, parent), null, null, Destroy, collectionCheck, defaultCapacity, maxSize)
        {

        }

        private static GameObject Create(GameObject template, Transform parent)
        {
            bool wasActive = template.activeSelf;
            template.SetActive(false);
            GameObject item = Object.Instantiate(template, parent);
            template.SetActive(wasActive);
            return item;
        }

        private static void Destroy(GameObject item)
        {
            Object.Destroy(item);
        }
    }

    public class GameObjectPool<T> : ObjectPool<T> where T : Component
    {
        public GameObjectPool(T template, Transform parent = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) : base(() => Create(template, parent), null, null, Destroy, collectionCheck, defaultCapacity, maxSize)
        {

        }

        private static T Create(T template, Transform parent)
        {
            bool wasActive = template.gameObject.activeSelf;
            template.gameObject.SetActive(false);
            T item = Object.Instantiate(template, parent);
            template.gameObject.SetActive(wasActive);
            return item;
        }

        private static void Destroy(T item)
        {
            Object.Destroy(item.gameObject);
        }
    }
}
