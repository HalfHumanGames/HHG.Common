using System.Collections;
using UnityEngine;

namespace HHG.Common
{
    public interface IActionAsync
    {
        IEnumerator DoAction(MonoBehaviour invoker);
    }
}