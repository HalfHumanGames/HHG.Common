using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    public static class CoroutineUtil
    {
        private class EmptyMonoBehaviour : MonoBehaviour { }

        private static MonoBehaviour _coroutiner;
        public static MonoBehaviour Coroutiner
        {
            get
            {
                if (_coroutiner == null && !isQuitting)
                {
                    GameObject go = new GameObject("Coroutiner");
                    Object.DontDestroyOnLoad(go);
                    _coroutiner = go.AddComponent<EmptyMonoBehaviour>();
                }
                return _coroutiner;
            }
        }

        private static bool isQuitting;

        static CoroutineUtil()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            Application.quitting += OnApplicationQuit;
        }

        private static void OnApplicationQuit()
        {
            Application.quitting -= OnApplicationQuit;
            isQuitting = true;
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            Coroutiner.StopAllCoroutines();
        }

        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return isQuitting ? null : Coroutiner.StartCoroutine(enumerator);
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            if (!isQuitting)
            {
                Coroutiner.StopCoroutine(coroutine);
            }
        }
    }
}