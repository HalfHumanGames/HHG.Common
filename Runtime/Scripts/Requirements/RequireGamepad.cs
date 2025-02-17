using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class RequireGamepad : IRequirement
    {
        public bool IsRequirementMet(MonoBehaviour invoker)
        {
            return Gamepad.current != null;
        }
    }
}