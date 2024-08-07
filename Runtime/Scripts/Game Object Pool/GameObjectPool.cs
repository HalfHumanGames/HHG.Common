using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    public partial class GameObjectPool
    {
        private static Dictionary<SubjectId, object> pools = new Dictionary<SubjectId, object>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            pools.Clear();
        }

        public static GameObjectPool GetPool(object id = null)
        {
            return GetPool(id, null);
        }

        public static GameObjectPool GetPool(GameObject template, Transform parent = null, bool collectionCheckEnabled = true, int defaultCapacity = 10, int maxPoolSize = 10000)
        {
            return GetPool(null, template, parent, collectionCheckEnabled, defaultCapacity, maxPoolSize);
        }

        public static GameObjectPool GetPool(object id, GameObject template, Transform parent = null, bool collectionCheckEnabled = true, int defaultCapacity = 10, int maxPoolSize = 10000)
        {
            SubjectId subjectId = new SubjectId(null, id);

            if (!pools.TryGetValue(subjectId, out object pool))
            {
                if (template != null)
                {
                    pool = new GameObjectPool(template, parent, collectionCheckEnabled, defaultCapacity, maxPoolSize);
                    pools[subjectId] = pool;
                }
                else
                {
                    Debug.LogError($"Pool with subject id '{subjectId}' does not exist and cannot be created since template is null.");
                }
            }

            return pool as GameObjectPool;
        }

        public static GameObjectPool<T> GetPool<T>(object id = null) where T : Component
        {
            return GetPool<T>(id, null);
        }

        public static GameObjectPool<T> GetPool<T>(T template, Transform parent = null, bool collectionCheckEnabled = true, int defaultCapacity = 10, int maxPoolSize = 10000) where T : Component
        {
            return GetPool(null, template, parent, collectionCheckEnabled, defaultCapacity, maxPoolSize);
        }

        public static GameObjectPool<T> GetPool<T>(object id, T template, Transform parent = null, bool collectionCheckEnabled = true, int defaultCapacity = 10, int maxPoolSize = 10000) where T : Component
        {
            SubjectId subjectId = new SubjectId(typeof(T), id);

            if (!pools.TryGetValue(subjectId, out object pool))
            {
                if (template != null)
                {
                    pool = new GameObjectPool<T>(template, parent, collectionCheckEnabled, defaultCapacity, maxPoolSize);
                    pools[subjectId] = pool;
                }
                else
                {
                    Debug.LogError($"Pool with subject id '{subjectId}' does not exist and cannot be created since template is null.");
                }
            }

            return pool as GameObjectPool<T>;
        }

        public static void ClearPool()
        {
            ClearPool(null);
        }

        public static void ClearPool(object id)
        {
            SubjectId subjectId = new SubjectId(null, id);

            if (pools.TryGetValue(subjectId, out object pool))
            {
                (pool as GameObjectPool).Clear();
            }
        }

        public static void ClearPool<T>() where T : Component
        {
            ClearPool<T>(null);
        }

        public static void ClearPool<T>(object id) where T : Component
        {
            SubjectId subjectId = new SubjectId(null, id);

            if (pools.TryGetValue(subjectId, out object pool))
            {
                (pool as GameObjectPool<T>).Clear();
            }
        }
    }
}
