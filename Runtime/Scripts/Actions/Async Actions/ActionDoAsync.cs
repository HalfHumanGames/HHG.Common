using System.Collections;
using UnityEngine;

namespace HHG.Common
{
    public class ActionDoAsync : IActionAsync
    {
        [SerializeReference, SerializeReferenceDropdown] private IAction action;

        public IEnumerator DoAction(MonoBehaviour invoker)
        {
            action?.DoAction(invoker);

            yield return null;
        }
    }
}