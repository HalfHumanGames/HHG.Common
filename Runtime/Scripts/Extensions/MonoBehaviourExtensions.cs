using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class MonoBehaviourExtensions
    {
        public static Coroutine StartCoroutine(this MonoBehaviour mono, IEnumerator enumerator, System.Action onComplete = null)
        {
            return mono.StartCoroutine(Run(enumerator, onComplete));

            static IEnumerator Run(IEnumerator enumerator, System.Action onComplete = null)
            {
                yield return enumerator;
                onComplete?.Invoke();
            }
        }

        public static void StopAndNullifyCoroutine(this MonoBehaviour mono, ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                mono.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        public static IEnumerator WaitForAll(this MonoBehaviour mono, IEnumerable<Coroutine> coroutines)
        {
            if (coroutines == null) yield break;

            foreach (Coroutine coroutine in coroutines)
            {
                yield return coroutine;
            }
        }

        public static IEnumerator WaitForSliced<T>(this MonoBehaviour mono, IEnumerable<T> items, int sliceSize, System.Action<T> action)
        {
            if (items == null) yield break;
            if (action == null) yield break;
            if (sliceSize <= 0) sliceSize = 1;

            int count = 0;
            foreach (T item in items)
            {
                action(item);
                count++;

                if (count >= sliceSize)
                {
                    count = 0;
                    yield return null;
                }
            }
        }
    }
}