using UnityEngine;

namespace HHG.Requirements
{
    public interface IRequirement
    {
        bool IsRequirementMet(MonoBehaviour invoker);
    }
}