using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HHG.Common.Runtime
{
    [Serializable]
    public class ActionEvent
    {
        public List<IActionBase> Actions => actions;

        [SerializeReference, SerializeReferenceDropdown] private List<IActionBase> actions = new List<IActionBase>();

        private event Action invokedEvent;
        private UnityEvent invokedUnityEvent;

        public Coroutine Invoke(MonoBehaviour invoker)
        {
            return invoker.StartCoroutine(InvokeRoutine(invoker));
        }

        public IEnumerator InvokeRoutine(MonoBehaviour invoker)
        {
            invokedEvent?.Invoke();
            invokedUnityEvent?.Invoke();
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
            invokedUnityEvent.AddListener(call);
        }

        public void RemoveListener(UnityAction call)
        {
            invokedUnityEvent.RemoveListener(call);
        }

        public void RemoveAllListeners()
        {
            actions.Clear();
            invokedEvent = null;
            invokedUnityEvent.RemoveAllListeners();
        }

        public static ActionEvent operator +(ActionEvent actionEvent, Action action)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException(nameof(actionEvent));
            }

            actionEvent.invokedEvent += action;

            return actionEvent;
        }

        public static ActionEvent operator -(ActionEvent actionEvent, Action action)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException(nameof(actionEvent));
            }

            actionEvent.invokedEvent -= action;

            return actionEvent;
        }
    }
}