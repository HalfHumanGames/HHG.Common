using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HHG.Common.Runtime
{
    public class ActionApplicationQuit : IAction
    {
        public void DoAction(MonoBehaviour invoker)
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}