using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class ActionApplicationQuit : IAction
    {
        public void Invoke(MonoBehaviour invoker)
        {
            ApplicationUtil.Quit();
        }
    }
}