using System;
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

        public static IEnumerator YieldParallel(this MonoBehaviour mono, IEnumerable<IEnumerator> enumerators)
        {
            if (enumerators == null) yield break;

            int running = 0;

            foreach (IEnumerator enumerator in enumerators)
            {
                mono.StartCoroutine(Run(enumerator));
            }

            while (running > 0) yield return null;

            IEnumerator Run(IEnumerator enumerator)
            {
                running++;
                yield return enumerator;
                running--;
            }
        }

        public static IEnumerator YieldSliced<T>(this MonoBehaviour mono, IEnumerable<T> items, int perFrame, Action<T> action)
        {
            if (items == null) yield break;
            if (action == null) yield break;
            if (perFrame <= 0) perFrame = 1;

            int count = 0;
            foreach (T item in items)
            {
                action(item);
                count++;

                if (count >= perFrame)
                {
                    count = 0;
                    yield return null;
                }
            }
        }
    }
}