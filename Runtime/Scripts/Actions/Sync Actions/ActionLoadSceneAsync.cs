using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    public class ActionLoadSceneAsync : IActionAsync
    {
        [SerializeField, Dropdown] private SceneNameAsset sceneName;

        public ActionLoadSceneAsync(SceneNameAsset scene)
        {
            sceneName = scene;
        }

        public IEnumerator InvokeAsync(MonoBehaviour invoker)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone) yield return new WaitForEndOfFrame();
        }
    }
}