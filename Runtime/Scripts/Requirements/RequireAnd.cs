using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class RequireAnd : IRequirement
    {
        [SerializeReference, SubclassSelector] private IRequirement[] requirements;

        public bool IsRequirementMet(MonoBehaviour invoker)
        {
            return requirements.All(r => r.IsRequirementMet(invoker));
        }
    }
}