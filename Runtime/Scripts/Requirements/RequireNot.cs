using UnityEngine;

namespace HHG.Common.Runtime
{
    public class RequireNot : IRequirement
    {
        [SerializeReference, SerializeReferenceDropdown] private IRequirement requirement;

        public bool IsRequirementMet(MonoBehaviour invoker)
        {
            return !requirement.IsRequirementMet(invoker);
        }
    }
}