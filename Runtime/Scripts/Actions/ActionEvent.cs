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

        [SerializeReference, SubclassSelector] protected List<IActionBase> actions = new List<IActionBase>();

        protected event Action plainEvent;
        protected UnityEvent unityEvent = new UnityEvent();

        public event Action Invoked {
            add => plainEvent += value;
            remove => plainEvent -= value;
        }

        public Coroutine Invoke(MonoBehaviour invoker)
        {
            return invoker.StartCoroutine(InvokeAsync(invoker));
        }

        public IEnumerator InvokeAsync(MonoBehaviour invoker)
        {
            plainEvent?.Invoke();
            unityEvent?.Invoke();

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

        public void RemoveListener(UnityAction call)
        {
            unityEvent.RemoveListener(call);
        }

        public virtual void RemoveAllListeners()
        {
            actions.Clear();
            plainEvent = null;
            unityEvent.RemoveAllListeners();
        }

        // Since we cannot invoke the event directly
        // in subclasses, we need to call this method
        protected void IssuePlainEvent()
        {
            plainEvent?.Invoke();
        }

        public static ActionEvent operator +(ActionEvent actionEvent, Action action)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException(nameof(actionEvent));
            }

            actionEvent.plainEvent += action;

            return actionEvent;
        }

        public static ActionEvent operator -(ActionEvent actionEvent, Action action)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException(nameof(actionEvent));
            }

            actionEvent.plainEvent -= action;

            return actionEvent;
        }
    }

    [Serializable]
    public class ActionEvent<T> : ActionEvent where T : MonoBehaviour
    {
        private event Action<T> plainEvent2;
        private UnityEvent<T> unityEvent2 = new UnityEvent<T>();

        public event Action<T> InvokedWithArgument
        {
            add => plainEvent2 += value;
            remove => plainEvent2 -= value;
        }

        public Coroutine Invoke(T invoker)
        {
            return invoker.StartCoroutine(InvokeAsync(invoker));
        }

        public IEnumerator InvokeAsync(T invoker)
        {
            // Since we cannot invoke the event directly
            // in subclasses, we need to call this method
            IssuePlainEvent();
            plainEvent2?.Invoke(invoker);
            unityEvent?.Invoke();
            unityEvent2?.Invoke(invoker);

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
            unityEvent2.AddListener(call);
        }

        public void RemoveListener(UnityAction<T> call)
        {
            unityEvent2.RemoveListener(call);
        }

        public override void RemoveAllListeners()
        {
            base.RemoveAllListeners();
            plainEvent2 = null;
            unityEvent2.RemoveAllListeners();
        }

        public static ActionEvent<T> operator +(ActionEvent<T> actionEvent, Action action)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException(nameof(actionEvent));
            }

            actionEvent.plainEvent += action;

            return actionEvent;
        }

        public static ActionEvent<T> operator -(ActionEvent<T> actionEvent, Action action)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException(nameof(actionEvent));
            }

            actionEvent.plainEvent -= action;

            return actionEvent;
        }

        public static ActionEvent<T> operator +(ActionEvent<T> actionEvent, Action<T> action)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException(nameof(actionEvent));
            }

            actionEvent.plainEvent2 += action;

            return actionEvent;
        }

        public static ActionEvent<T> operator -(ActionEvent<T> actionEvent, Action<T> action)
        {
            if (actionEvent == null)
            {
                throw new ArgumentNullException(nameof(actionEvent));
            }

            actionEvent.plainEvent2 -= action;

            return actionEvent;
        }
    }
}