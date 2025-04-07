#if UNITY_EDITOR
using UnityEditor;
#else
using UnityEngine;
#endif

namespace HHG.Common.Runtime
{
    public static class ApplicationUtil
    {
        public static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}