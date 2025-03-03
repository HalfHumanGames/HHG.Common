using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    [Serializable]
    public class ActionLoadSceneAsync : IActionAsync
    {
        [SerializeField] private SerializedScene scene;

        private Action onLoaded;

        public ActionLoadSceneAsync()
        {

        }

        public ActionLoadSceneAsync(SerializedScene scene, Action onLoaded = null)
        {
            this.scene = scene;
            this.onLoaded = onLoaded;
        }

        public IEnumerator InvokeAsync(MonoBehaviour invoker)
        {
            if (scene == null)
            {
                Debug.LogError("Scene cannot be null.", invoker);
                yield break;
            }

            if (!scene.CanBeLoaded)
            {
                Debug.LogError("Scene cannot be loaded.", invoker);
                yield break;
            }

            AsyncOperation op = SceneManager.LoadSceneAsync(scene.BuildIndex);
            op.completed += OnCompleted;
            while (!op.isDone) yield return new WaitForEndOfFrame();  
        }

        private void OnCompleted(AsyncOperation op)
        {
            onLoaded?.Invoke();
        }
    }
}