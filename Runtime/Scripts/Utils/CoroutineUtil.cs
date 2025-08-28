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

        public static Coroutine StartCoroutine(IEnumerator enumerator, System.Action onComplete = null)
        {
            return !isQuitting && Coroutiner != null ? Coroutiner.StartCoroutine(enumerator, onComplete) : null;
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
            if (!isQuitting && Coroutiner != null)
            {
                Coroutiner.StopAndNullifyCoroutine(ref coroutine);
            }
        }

        public static IEnumerator YieldParallel(IEnumerable<IEnumerator> enumerators)
        {
            yield return !isQuitting && Coroutiner != null ? Coroutiner.YieldParallel(enumerators) : null;
        }

        public static IEnumerator YieldSliced<T>(IEnumerable<T> items, int perFrame, System.Action<T> action)
        {
            yield return !isQuitting && Coroutiner != null ? Coroutiner.YieldSliced(items, perFrame, action) : null;
        }
    }
}