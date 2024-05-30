using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [Serializable]
    public class ActionEvent
    {
        public List<IActionBase> Actions => actions;

        [SerializeReference, SerializeReferenceDropdown] private List<IActionBase> actions = new List<IActionBase>();

        public Coroutine Invoke(MonoBehaviour invoker)
        {
            return invoker.StartCoroutine(InvokeRoutine(invoker));
        }

        public IEnumerator InvokeRoutine(MonoBehaviour invoker)
        {
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
    }
}