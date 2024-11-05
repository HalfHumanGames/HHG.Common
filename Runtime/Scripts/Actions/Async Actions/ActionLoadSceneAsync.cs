using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    [Serializable]
    public class ActionLoadSceneAsync : IActionAsync
    {
        [SerializeField, Dropdown] private SceneNameAsset sceneName;

        private Action onLoaded;

        public ActionLoadSceneAsync()
        {

        }

        public ActionLoadSceneAsync(SceneNameAsset sceneName, Action onLoaded = null)
        {
            this.sceneName = sceneName;
            this.onLoaded = onLoaded;
        }

        public IEnumerator InvokeAsync(MonoBehaviour invoker)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.completed += OnCompleted;
            while (!op.isDone) yield return new WaitForEndOfFrame();  
        }

        private void OnCompleted(AsyncOperation op)
        {
            onLoaded?.Invoke();
        }
    }
}