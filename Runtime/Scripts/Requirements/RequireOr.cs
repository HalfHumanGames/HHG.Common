using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class RequireOr : IRequirement
    {
        [SerializeReference, SubclassSelector] private IRequirement[] requirements;

        public bool IsRequirementMet(MonoBehaviour invoker)
        {
            return requirements.Any(r => r.IsRequirementMet(invoker));
        }
    }
}