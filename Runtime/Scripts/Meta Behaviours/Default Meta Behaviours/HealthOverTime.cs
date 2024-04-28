using UnityEngine;

namespace HHG.Common.Runtime
{
    public class HealthOverTime : MetaBehaviour, IAggregatable, IAggregatable<HealthOverTime>
    {
        [SerializeField] private float frequency;
        [SerializeField] private float amount;

        private IHealth health;
        private float timer = .01f;

        public override void Start()
        {
            health = gameObject.GetComponent<IHealth>();
        }

        public override void Update()
        {
            if (health != null)
            {
                if (timer > 0f)
                {
                    timer -= Time.deltaTime;

                    if (timer <= 0f)
                    {
                        timer = frequency;
                        
                        if (amount < 0f)
                        {
                            // Use absolute since can't take negative damage
                            health.TakeDamage(Mathf.Abs(amount));
                        }
                        else if (amount > 0f)
                        {
                            health.Heal(amount);
                        }
                    }
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