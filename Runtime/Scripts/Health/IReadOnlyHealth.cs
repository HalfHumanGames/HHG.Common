using UnityEngine;
using UnityEngine.Events;

namespace HHG.Common.Runtime
{
    public interface IReadOnlyHealth
    {
        public MonoBehaviour Mono { get; }
        public float HealthValue { get; }
        public float HealthPerc { get; }
        public float PreviousHealthPerc { get; }
        public float PreviousHealthValue { get; }
        public float PreviousHealthValueDelta { get; }
        public bool IsAlive { get; }
        public bool IsDead { get; }
    }

    public interface IReadOnlyHealthWithEvents : IReadOnlyHealth
    {
        public UnityEvent<IHealth> OnHit { get; }
        public UnityEvent<IHealth> OnHeal { get; }
        public UnityEvent<IHealth> OnDied { get; }
        public UnityEvent<IHealth> OnHealthUpdated { get; }
    }
}