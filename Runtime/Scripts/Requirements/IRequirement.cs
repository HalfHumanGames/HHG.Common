using UnityEngine;

namespace HHG.Common.Runtime
{
    public interface IRequirement
    {
        bool IsRequirementMet(MonoBehaviour invoker);
    }
}