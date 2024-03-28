using UnityEngine;

namespace HHG.Common
{
    public interface IAction
    {
        void DoAction(MonoBehaviour invoker);
    }
}