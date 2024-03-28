using UnityEngine;

namespace HHG.Common.Runtime
{
    public interface IReadOnlyHealth
    {
        public MonoBehaviour Mono { get; }

        public float HealthValue { get; }
        public float HealthPerc { get; }
    }
}