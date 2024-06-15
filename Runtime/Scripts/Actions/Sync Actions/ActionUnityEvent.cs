using UnityEngine;
using UnityEngine.Events;

namespace HHG.Common.Runtime
{
    public class ActionUnityEvent : IAction
    {
        [SerializeField] private UnityEvent unityEvent = new UnityEvent();

        public void Invoke(MonoBehaviour invoker)
        {
            unityEvent?.Invoke();
        }
    }
}