using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract partial class GameObjectPoolBase<T> : ObjectPool<T> where T : Object
    {
        private List<ReleaseInfo> releaseInfos = new List<ReleaseInfo>();
        private int lastCheck;
        private bool isDirty;

        public GameObjectPoolBase(T template, Transform parent = null, bool collectionCheckEnabled = true, int defaultCapacity = 10, int maxPoolSize = 10000, bool prewarm = false) : base(() => default)
        {
            list = new List<T>(defaultCapacity);
            create = () => CheckDelayedReleasesThenCreate(template, parent);
            maxSize = Mathf.Max(maxPoolSize, 1);
            onGet = null;
            onRelease = null;
            destroy = Destroy;
            collectionCheck = collectionCheckEnabled;

            if (prewarm)
            {
                countAll = list.Capacity;

                for (int i = 0; i < countAll; i++)
                {
                    list.Add(create());
                }
            }
        }

        protected abstract T Create(T template, Transform parent);
        protected abstract void Destroy(T item);

        protected T CheckDelayedReleasesThenCreate(T item, Transform parent)
        {
            CheckDelayedReleases();

            T val;
            if (list.Count == 0)
            {
                val = Create(item, parent);
            }
            else
            {
                int index = list.Count - 1;
                val = list[index];
                list.RemoveAt(index);
                countAll--; // To void inc in Get
            }

            return val;
        }

        public void Release(T item, int frames)
        {
            isDirty = true;
            releaseInfos.Add(new ReleaseInfo(item, Time.frameCount + frames));
        }

        public void Release(T item, float seconds, bool unscaled = false)
        {
            isDirty = true;
            releaseInfos.Add(new ReleaseInfo(item, (unscaled ? Time.unscaledTime : Time.time) + seconds));
        }

        private void CheckDelayedReleases()
        {
            if (lastCheck != Time.frameCount || isDirty)
            {
                lastCheck = Time.frameCount;
                isDirty = false;

                for (int i = 0; i < releaseInfos.Count; i++)
                {
                    if (releaseInfos[i].CanRelease(out T item))
                    {
                        Release(item);
                        releaseInfos.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    } 
}