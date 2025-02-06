using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class ActionOpenUrl : IAction
    {
        [SerializeField] private string url;

        public void Invoke(MonoBehaviour invoker)
        {
            Application.OpenURL(url);
        }
    }
}