using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    [Serializable]
    public class ActionLoadSceneAsync : IActionAsync
    {
        private enum Mode
        {
            ByName,
            ByIndex
        }

        [SerializeField] private Mode mode;
        [SerializeField, ShowIf(nameof(mode), Mode.ByName), Dropdown] private SceneNameAsset sceneName;
        [SerializeField, ShowIf(nameof(mode), Mode.ByIndex)] private int sceneIndex;

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
            AsyncOperation op = mode == Mode.ByName ?
                SceneManager.LoadSceneAsync(sceneName) :
                SceneManager.LoadSceneAsync(sceneIndex);

            op.completed += OnCompleted;
            while (!op.isDone) yield return new WaitForEndOfFrame();  
        }

        private void OnCompleted(AsyncOperation op)
        {
            onLoaded?.Invoke();
        }
    }
}