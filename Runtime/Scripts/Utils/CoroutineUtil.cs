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
                if (_coroutiner == null)
                {
                    GameObject go = new GameObject("Coroutiner");
                    Object.DontDestroyOnLoad(go);
                    _coroutiner = go.AddComponent<EmptyMonoBehaviour>();
                }
                return _coroutiner;
            }
        }

        static CoroutineUtil()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            Coroutiner.StopAllCoroutines();
        }

        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return Coroutiner.StartCoroutine(enumerator);
        }
    }
}