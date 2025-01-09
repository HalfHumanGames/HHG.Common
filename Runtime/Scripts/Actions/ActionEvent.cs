using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HHG.Common.Runtime
{
    [Serializable]
    public class ActionEvent<T> where T : MonoBehaviour
    {
        public List<IActionBase> Actions => actions;

        [SerializeReference, SubclassSelector] private List<IActionBase> actions = new List<IActionBase>();

        private event Action<T> invokedEvent;
        private UnityEvent<T> invokedUnityEvent = new UnityEvent<T>();

        public Coroutine Invoke(T invoker)
        {
            return invoker.StartCoroutine(InvokeRoutine(invoker));
        }

        public IEnumerator InvokeRoutine(T invoker)
        {
            invokedEvent?.Invoke(invoker);
            invokedUnityEvent?.Invoke(invoker);

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

        public void AddListener(UnityAction<T> call)
        {
            invokedUnityEvent.AddListener(call);
        }

        public void RemoveListener(UnityAction<T> call)
        {
            invokedUnityEvent.RemoveListener(call);
        }

        public void RemoveAllListeners()
        {
            actions.Clear();
            invokedEvent = null;
            invokedUnityEvent.RemoveAllListeners();
        }

        public static ActionEvent<T> operator +(ActionEvent<T> actionEvent, Action<T> action)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException(nameof(actionEvent));
            }

            actionEvent.invokedEvent += action;

            return actionEvent;
        }

        public static ActionEvent<T> operator -(ActionEvent<T> actionEvent, Action<T> action)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException(nameof(actionEvent));
            }

            actionEvent.invokedEvent -= action;

            return actionEvent;
        }
    }

    public class ActionEvent : ActionEvent<MonoBehaviour>
    {

    }
}