using System.Collections;
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

        private static MonoBehaviour coroutiner;
        public static MonoBehaviour Coroutiner
        {
            get
            {
                if (coroutiner == null && !isQuitting)
                {
                    GameObject go = new GameObject("Coroutiner");
                    Object.DontDestroyOnLoad(go);
                    coroutiner = go.AddComponent<EmptyMonoBehaviour>();
                }
                return coroutiner;
            }
        }

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

        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return isQuitting ? null : Coroutiner?.StartCoroutine(enumerator);
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            if (!isQuitting)
            {
                Coroutiner?.StopCoroutine(coroutine);
            }
        }
    }
}