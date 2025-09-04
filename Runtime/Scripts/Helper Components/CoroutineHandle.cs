using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class CoroutineHandle : System.IDisposable
    {
        private static bool locked = false;

        public bool IsDone => isDone && children.TrueForAll(c => c.IsDone);

        private MonoBehaviour owner;
        private Coroutine coroutine;
        private bool isDone;
        private List<CoroutineHandle> children = new List<CoroutineHandle>();

        public CoroutineHandle()
        {
            if (locked)
            {
                throw new System.InvalidOperationException("CoroutineHandle instances must be created using CoroutineHandle.GetFromPool()");
            }
        }

        public static CoroutineHandle GetFromPool()
        {
            locked = false;
            CoroutineHandle handle = Pool.Get<CoroutineHandle>();
            locked = true;
            handle.owner = null;
            handle.coroutine = null;
            handle.isDone = true;
            handle.children.Clear();
            return handle;
        }

        public CoroutineHandle StartCoroutine(MonoBehaviour owner, IEnumerator routine)
        {
            if (owner == null) throw new System.ArgumentNullException(nameof(owner), "Owner MonoBehaviour cannot be null.");
            if (routine == null) throw new System.ArgumentNullException(nameof(routine), "Coroutine routine cannot be null.");

            CoroutineHandle child = GetFromPool();
            child.owner = owner;
            child.isDone = false;
            child.StartCoroutineInternal(routine);
            children.Add(child);
            return child;
        }

        private void StartCoroutineInternal(IEnumerator routine)
        {
            coroutine = owner.StartCoroutine(Run(routine));
        }

        private IEnumerator Run(IEnumerator routine)
        {
            yield return routine;
            isDone = true;
        }

        public void Stop()
        {
            if (!isDone && owner != null && coroutine != null)
            {
                owner.StopCoroutine(coroutine);
            }

            foreach (CoroutineHandle child in children)
            {
                child.Stop();
            }

            isDone = true;
        }

        public void ReleaseToPool()
        {
            Stop();

            foreach (CoroutineHandle child in children)
            {
                child.ReleaseToPool();
            }

            owner = null;
            coroutine = null;
            isDone = true;
            children.Clear();

            Pool.Release(this);
        }

        public void Dispose()
        {
            ReleaseToPool();
        }
    }
}
