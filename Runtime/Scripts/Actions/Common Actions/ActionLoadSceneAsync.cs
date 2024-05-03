using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    public class ActionLoadSceneAsync : IActionAsync
    {

#if UNITY_EDITOR
        [Dropdown(typeof(SceneAsset))]
#endif
        [SerializeField] private Object scene;

        public IEnumerator DoActionAsync(MonoBehaviour invoker)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(scene.name);

            while (!op.isDone) yield return new WaitForEndOfFrame();
        }
    }
}