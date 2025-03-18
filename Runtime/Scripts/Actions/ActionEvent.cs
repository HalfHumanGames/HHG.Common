using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HHG.Common.Runtime
{
    [Serializable]
    public class ActionEvent : ActionEvent<MonoBehaviour> { }

    [Serializable]
    public class ActionEvent<T> where T : MonoBehaviour
    {
        public List<IActionBase> Actions => actions;

        [SerializeReference, SubclassSelector] private List<IActionBase> actions = new List<IActionBase>();

        private UnityEvent unityEvent = new UnityEvent();
        private UnityEvent<T> unityEventWithArgument = new UnityEvent<T>();

        public ActionEvent()
        {

        }

        public ActionEvent(UnityAction action)
        {
            unityEvent.AddListener(action);
        }

        public ActionEvent(UnityAction<T> action)
        {
            unityEventWithArgument.AddListener(action);
        }

        public Coroutine Invoke(T invoker)
        {
            return invoker.StartCoroutine(InvokeAsync(invoker));
        }

        public IEnumerator InvokeAsync(T invoker)
        {
            unityEvent?.Invoke();
            unityEventWithArgument?.Invoke(invoker);

            foreach (IActionBase action in actions)
            {
                if (action is IAction syncAction)
                {
                    syncAction.Invoke(invoker);
                }
                else if (action is IActionAsync asyncAction)
                {
                    yield return asyncAction.InvokeAsync(invoker);
                }
            }
        }

        public void AddListener(UnityAction call)
        {
            unityEvent.AddListener(call);
        }

        public void AddListener(UnityAction<T> call)
        {
            unityEventWithArgument.AddListener(call);
        }

        public void RemoveListener(UnityAction call)
        {
            unityEvent.RemoveListener(call);
        }

        public void RemoveListener(UnityAction<T> call)
        {
            unityEventWithArgument.RemoveListener(call);
        }

        public void RemoveAllListeners()
        {
            actions.Clear();
            unityEvent.RemoveAllListeners();
            unityEventWithArgument.RemoveAllListeners();
        }
    }
}