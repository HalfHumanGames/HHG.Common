using UnityEngine;
using UnityEngine.Events;

namespace HHG.Common.Runtime
{
    public class ActionInvokeUnityEvent : IAction
    {
        [SerializeField] private UnityEvent invoke;

        public void DoAction(MonoBehaviour invoker)
        {
            invoke?.Invoke();
        }
    }
}