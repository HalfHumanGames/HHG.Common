using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract partial class GameObjectPoolBase<T> : ObjectPool<T> where T : Object
    {
        private List<ReleaseInfo> releaseInfos = new List<ReleaseInfo>();
        private int lastCheck;
        private bool isDirty;
        private T template;
        private Transform parent;

        public GameObjectPoolBase(T template, Transform parent = null, bool collectionCheckEnabled = true, int defaultCapacity = 10, int maxPoolSize = 10000, bool prewarm = false) : base(() => default)
        {
            this.template = template;
            this.parent = parent;
            list = new List<T>(defaultCapacity);
            create = CheckDelayedReleasesThenCreate;
            maxSize = Mathf.Max(maxPoolSize, 1);
            onGet = null;
            onRelease = null;
            destroy = Destroy;
            collectionCheck = collectionCheckEnabled;

            if (prewarm)
            {
                Prewarm();
            }
        }

        protected abstract T Create(T template, Transform parent);
        protected abstract void Destroy(T item);

        protected T CheckDelayedReleasesThenCreate()
        {
            CheckDelayedReleases();

            T val;
            if (list.Count == 0)
            {
                val = Create(template, parent);
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

        public override void Prewarm()
        {
            int add = Mathf.Max(list.Capacity - countAll, 0);

            for (int i = 0; i < add; i++)
            {
                list.Add(Create(template, parent));
            }
            
            countAll += add;
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