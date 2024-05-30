using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public interface IActionAsync : IActionBase
    {
        IEnumerator InvokeAsync(MonoBehaviour invoker);
    }
}