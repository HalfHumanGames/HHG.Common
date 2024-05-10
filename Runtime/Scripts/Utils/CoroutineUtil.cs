using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    public static class CoroutineUtil
    {
        public class Coroutiner : MonoBehaviour { }

        private static MonoBehaviour _coroutiner;
        private static MonoBehaviour coroutiner
        {
            get
            {
                if (_coroutiner == null)
                {
                    GameObject go = new GameObject("Coroutiner");
                    Object.DontDestroyOnLoad(go);
                    _coroutiner = go.AddComponent<Coroutiner>();
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
            coroutiner.StopAllCoroutines();
        }

        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return coroutiner.StartCoroutine(enumerator);
        }
    }
}