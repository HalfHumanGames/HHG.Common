using UnityEngine.Events;

namespace HHG.Common.Runtime
{
    public interface IHealth : IReadOnlyHealth
    {
        public UnityEvent<IHealth> OnHit { get; }
        public UnityEvent<IHealth> OnHeal { get; }
        public UnityEvent<IHealth> OnDied { get; }

        public void Initialize(float health);
        public void TakeDamage(float amount);
        public void Heal(float amount = -1);
    }
}