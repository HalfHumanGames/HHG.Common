using UnityEngine;
using UnityEngine.Events;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class ActionUnityEvent : IAction
    {
        [SerializeField] private UnityEvent unityEvent = new UnityEvent();

        public void Invoke(MonoBehaviour invoker)
        {
            unityEvent?.Invoke();
        }
    }
}