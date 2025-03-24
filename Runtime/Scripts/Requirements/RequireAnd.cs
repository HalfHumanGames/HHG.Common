using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class RequireAnd : IRequirement
    {
        [SerializeReference, SubclassSelector] private List<IRequirement> requirements = new List<IRequirement>();

        public bool IsRequirementMet(MonoBehaviour invoker)
        {
            return requirements.All(r => r.IsRequirementMet(invoker));
        }
    }
}