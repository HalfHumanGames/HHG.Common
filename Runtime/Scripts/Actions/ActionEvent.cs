using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class ActionEvent : ActionEvent<MonoBehaviour, object>
    {
        public ActionEvent()
        {

        }

        public ActionEvent(System.Action action) : base(action)
        {
            
        }

        public ActionEvent(System.Action<MonoBehaviour> action) : base(action)
        {
            
        }

        public ActionEvent(System.Action<MonoBehaviour, object> action) : base(action)
        {

        }
    }

    [System.Serializable]
    public class ActionEvent<TInvoker> : ActionEvent<TInvoker, object> where TInvoker : MonoBehaviour
    {
        public ActionEvent()
        {

        }

        public ActionEvent(System.Action action) : base(action)
        {

        }

        public ActionEvent(System.Action<TInvoker> action) : base(action)
        {

        }

        public ActionEvent(System.Action<TInvoker, object> action) : base(action)
        {

        }
    }

    [System.Serializable]
    public class ActionEvent<TInvoker, TContext> where TInvoker : MonoBehaviour
    {
        public List<IActionBase> Actions => actions;

        [SerializeReference, ReferencePicker] private List<IActionBase> actions = new List<IActionBase>();

        private event System.Action invoked1;
        private event System.Action<TInvoker> invoked2;
        private event System.Action<TInvoker, TContext> invoked3;

        public ActionEvent()
        {

        }

        public ActionEvent(System.Action action)
        {
            invoked1 += action;
        }

        public ActionEvent(System.Action<TInvoker> action)
        {
            invoked2 += action;
        }

        public ActionEvent(System.Action<TInvoker, TContext> action)
        {
            invoked3 += action;
        }

        public Coroutine Invoke(TInvoker invoker, TContext context = default)
        {
            return invoker.StartCoroutine(InvokeAsync(invoker, context));
        }

        public IEnumerator InvokeAsync(TInvoker invoker, TContext context = default)
        {
            invoked1?.Invoke();
            invoked2?.Invoke(invoker);
            invoked3?.Invoke(invoker, context);

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

        public void AddListener(System.Action action)
        {
            invoked1 += action;
        }

        public void AddListener(System.Action<TInvoker> action)
        {
            invoked2 += action;
        }

        public void AddListener(System.Action<TInvoker, TContext> action)
        {
            invoked3 += action;
        }

        public void RemoveListener(System.Action action)
        {
            invoked1 -= action;
        }

        public void RemoveListener(System.Action<TInvoker> action)
        {
            invoked2 -= action;
        }

        public void RemoveListener(System.Action<TInvoker, TContext> action)
        {
            invoked3 -= action;
        }

        public void RemoveAllListeners()
        {
            actions.Clear();
            invoked1 = null;
            invoked2 = null;
            invoked3 = null;
        }
    }
}