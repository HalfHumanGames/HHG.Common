using UnityEngine;

namespace HHG.Common.Runtime
{
    public partial class GameObjectPool : GameObjectPoolBase<GameObject>
    {
        public GameObjectPool(GameObject template, Transform parent = null, bool collectionCheckEnabled = true, int defaultCapacity = 10, int maxPoolSize = 10000, bool prewarm = false) : base(template, parent, collectionCheckEnabled, defaultCapacity, maxPoolSize, prewarm)
        {

        }

        protected override GameObject Create(GameObject template, Transform parent)
        {
            bool wasActive = template.activeSelf;
            if (template.activeSelf)
            {
                template.SetActive(false);
            }
            GameObject item = Object.Instantiate(template, parent);
            if (wasActive)
            {
                template.SetActive(wasActive);
            }
            return item;
        }

        protected override void Destroy(GameObject item)
        {
            Object.Destroy(item);
        }
    }

    public class GameObjectPool<T> : GameObjectPoolBase<T> where T : Component
    {
        public GameObjectPool(T template, Transform parent = null, bool collectionCheckEnabled = true, int defaultCapacity = 10, int maxPoolSize = 10000, bool prewarm = false) : base(template, parent, collectionCheckEnabled, defaultCapacity, maxPoolSize, prewarm)
        {

        }

        protected override T Create(T template, Transform parent)
        {
            bool wasActive = template.gameObject.activeSelf;
            if (template.gameObject.activeSelf)
            {
                template.gameObject.SetActive(false);
            }
            T item = Object.Instantiate(template, parent);
            if (wasActive)
            {
                template.gameObject.SetActive(wasActive);
            }
            return item;
        }

        protected override void Destroy(T item)
        {
            Object.Destroy(item.gameObject);
        }
    }
}