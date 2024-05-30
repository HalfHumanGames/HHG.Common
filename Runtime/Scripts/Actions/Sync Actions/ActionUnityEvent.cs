using UnityEngine;
using UnityEngine.Events;

namespace HHG.Common.Runtime
{
    public class ActionUnityEvent : IAction
    {
        [SerializeField] private UnityEvent unityEvent;

        public void Invoke(MonoBehaviour invoker)
        {
            unityEvent?.Invoke();
        }
    }
}