using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    public static class CoroutineUtil
    {
        private class EmptyMonoBehaviour : MonoBehaviour { }

        public static MonoBehaviour Coroutiner
        {
            get
            {
                if (coroutiner == null && Application.isPlaying && !isQuitting)
                {
                    GameObject go = GameObject.Find("Coroutiner");

                    if (go == null)
                    {
                        go = new GameObject("Coroutiner");
                    }

                    Object.DontDestroyOnLoad(go);

                    coroutiner = go.GetOrAddComponent<EmptyMonoBehaviour>();
                }
                return coroutiner;
            }
        }

        public static Coroutine Empty => StartCoroutine(YieldBreak());

        private static MonoBehaviour coroutiner;
        private static bool isQuitting;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            Application.quitting -= OnApplicationQuit;
            Application.quitting += OnApplicationQuit;
#if UNITY_EDITOR
            EditorApplication.quitting -= OnApplicationQuit;
            EditorApplication.quitting += OnApplicationQuit;
#endif
        }

        private static void OnApplicationQuit()
        {
            // Don't create if doesn't exist
            coroutiner?.StopAllCoroutines();
            isQuitting = true;
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            // Don't create if doesn't exist
            coroutiner?.StopAllCoroutines();
        }

        private static IEnumerator YieldBreak()
        {
            yield break;
        }

        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return isQuitting || Coroutiner == null ? null : Coroutiner.StartCoroutine(enumerator);
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            if (!isQuitting && Coroutiner != null && coroutine != null)
            {
                Coroutiner.StopCoroutine(coroutine);
            }
        }

        public static void StopAndNullifyCoroutine(ref Coroutine coroutine)
        {
            if (!isQuitting && Coroutiner != null && coroutine != null)
            {
                Coroutiner.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        public static void ReplaceCoroutine(ref Coroutine coroutine, Coroutine val)
        {
            StopAndNullifyCoroutine(ref coroutine);
            coroutine = val;
        }

        public static void ReplaceCoroutine(ref Coroutine coroutine, IEnumerator enumerator)
        {
            StopAndNullifyCoroutine(ref coroutine);
            coroutine = StartCoroutine(enumerator);
        }

        public static IEnumerator YieldAllParallel(IEnumerable<IEnumerator> enumerators)
        {
            int running = 0;

            foreach (IEnumerator enumerator in enumerators)
            {
                StartCoroutine(Run(enumerator));
            }

            while (running > 0) yield return null;

            IEnumerator Run(IEnumerator enumerator)
            {
                running++;
                yield return enumerator;
                running--;
            }
        }
    }
}