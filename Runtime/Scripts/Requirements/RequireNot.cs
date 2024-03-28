using UnityEngine;

namespace HHG.Requirements
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