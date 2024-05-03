using UnityEngine;

namespace HHG.Common.Runtime
{
    public class HealthOverTime : MetaBehaviour, IAggregatable, IAggregatable<HealthOverTime>
    {
        [SerializeField] private float frequency;
        [SerializeField] private float amount;

        private IHealth health;

        public override void Start()
        {
            health = gameObject.GetComponent<IHealth>();
        }

        public override void Update()
        {
            if (health != null)
            {
                float actual = 1f / frequency * amount * Time.deltaTime;
                if (actual < 0f)
                {
                    // Negate since can't take negative damage
                    health.TakeDamage(-actual);
                }
                else if (actual > 0f)
                {
                    health.Heal(actual);
                }
            }
        }

        public object Aggregate(object other)
        {
            return Aggregate(other as HealthOverTime);
        }

        public HealthOverTime Aggregate(HealthOverTime other)
        {
            amount += other.amount;
            return this;
        }
    }
}