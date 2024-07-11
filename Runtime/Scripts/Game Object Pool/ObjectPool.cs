using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace HHG.Common.Runtime
{
    // This is based on Unity's ObjectPool class, but with protected fields to allow for the creation of subclasses
    public class ObjectPool<T> : IDisposable, IObjectPool<T> where T : class
    {
        public int CountAll => countAll;
        public int CountActive => CountAll - CountInactive;
        public int CountInactive => list.Count;

        protected Func<T> create;
        protected Action<T> onGet;
        protected Action<T> onRelease;
        protected Action<T> destroy;
        protected int maxSize;
        protected List<T> list;
        protected bool collectionCheck;
        protected int countAll;

        public ObjectPool(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null, bool collectionCheckEnabled = true, int defaultCapacity = 10, int maxPoolSize = 10000)
        {
            if (createFunc == null)
            {
                throw new ArgumentNullException("createFunc");
            }

            if (maxPoolSize <= 0)
            {
                throw new ArgumentException("Max Size must be greater than 0", "maxSize");
            }

            list = new List<T>(defaultCapacity);
            create = createFunc;
            maxSize = maxPoolSize;
            onGet = actionOnGet;
            onRelease = actionOnRelease;
            destroy = actionOnDestroy;
            collectionCheck = collectionCheckEnabled;
        }

        public T Get()
        {
            T val;
            if (list.Count == 0)
            {
                val = create();
                countAll++;
            }
            else
            {
                int index = list.Count - 1;
                val = list[index];
                list.RemoveAt(index);
            }

            onGet?.Invoke(val);
            return val;
        }

        public PooledObject<T> Get(out T v)
        {
            return new PooledObject<T>(v = Get(), this);
        }

        public void Release(T element)
        {
            if (collectionCheck && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (element == list[i])
                    {
                        throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");
                    }
                }
            }

            onRelease?.Invoke(element);
            if (CountInactive < maxSize)
            {
                list.Add(element);
                return;
            }

            countAll--;
            destroy?.Invoke(element);
        }

        public void Clear()
        {
            if (destroy != null)
            {
                foreach (T item in list)
                {
                    destroy(item);
                }
            }

            list.Clear();
            countAll = 0;
        }

        public void Dispose()
        {
            Clear();
        }
    }
}