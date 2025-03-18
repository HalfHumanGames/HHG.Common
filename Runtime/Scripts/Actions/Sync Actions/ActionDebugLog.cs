using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class ActionDebugLog : IAction
    {
        [SerializeField] private string message;

        public void Invoke(MonoBehaviour invoker)
        {
            Debug.Log(message);
        }
    }
}