using UnityEngine;

namespace HHG.Common.Runtime
{
    public interface IAction
    {
        void DoAction(MonoBehaviour invoker);
    }
}