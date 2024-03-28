using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public interface IActionAsync
    {
        IEnumerator DoAction(MonoBehaviour invoker);
    }
}