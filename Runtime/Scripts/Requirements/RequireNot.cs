using UnityEngine;

namespace HHG.Common.Runtime
{
    public class RequireNot : IRequirement
    {
        [SerializeReference, ReferencePicker] private IRequirement requirement;

        public bool IsRequirementMet(MonoBehaviour invoker)
        {
            return !requirement.IsRequirementMet(invoker);
        }
    }
}