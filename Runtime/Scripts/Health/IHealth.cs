namespace HHG.Common.Runtime
{
    public interface IHealth : IReadOnlyHealthWithEvents
    {
        public void Initialize(float health);
        public void TakeDamage(float amount);
        public void Heal(float amount = -1);
    }
}