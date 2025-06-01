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

        private event System.Func<IEnumerator> coroutine1;
        private event System.Func<TInvoker, IEnumerator> coroutine2;
        private event System.Func<TInvoker, TContext, IEnumerator> coroutine3;

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

        public ActionEvent(System.Func<IEnumerator> func)
        {
            coroutine1 += func;
        }

        public ActionEvent(System.Func<TInvoker, IEnumerator> func)
        {
            coroutine2 += func;
        }

        public ActionEvent(System.Func<TInvoker, TContext, IEnumerator> func)
        {
            coroutine3 += func;
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

            if (coroutine1 != null) yield return coroutine1();
            if (coroutine2 != null) yield return coroutine2(invoker);
            if (coroutine3 != null) yield return coroutine3(invoker, context);

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

        public void AddListener(System.Func<IEnumerator> func)
        {
            coroutine1 += func;
        }

        public void AddListener(System.Func<TInvoker, IEnumerator> func)
        {
            coroutine2 += func;
        }

        public void AddListener(System.Func<TInvoker, TContext, IEnumerator> func)
        {
            coroutine3 += func;
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

        public void RemoveListener(System.Func<IEnumerator> func)
        {
            coroutine1 -= func;
        }

        public void RemoveListener(System.Func<TInvoker, IEnumerator> func)
        {
            coroutine2 -= func;
        }

        public void RemoveListener(System.Func<TInvoker, TContext, IEnumerator> func)
        {
            coroutine3 -= func;
        }

        public void RemoveAllListeners()
        {
            actions.Clear();
            invoked1 = null;
            invoked2 = null;
            invoked3 = null;
            coroutine1 = null;
            coroutine2 = null;
            coroutine3 = null;
        }
    }
}