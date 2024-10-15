using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class ActionApplicationQuit : IAction
    {
        public void Invoke(MonoBehaviour invoker)
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}