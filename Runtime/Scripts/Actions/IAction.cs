using UnityEngine;

namespace HHG.Common.Runtime
{
    public interface IAction : IActionBase
    {
        void Invoke(MonoBehaviour invoker);
    }
}