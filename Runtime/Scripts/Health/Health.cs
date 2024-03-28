using UnityEngine;
using UnityEngine.Events;

namespace HHG.Common
{
    public class Health : MonoBehaviour, IHealth
    {
        public MonoBehaviour Mono => this;
        public float HealthValue => health;
        public float HealthPerc => maxHealth == 0f ? 1f : health / maxHealth;
        public UnityEvent<IHealth> OnHit => onHit;
        public UnityEvent<IHealth> OnHeal => onHeal;
        public UnityEvent<IHealth> OnDied => onDied;

        [SerializeField] private float health;
        [SerializeField] private float healthOverTime;
        [SerializeField] private UnityEvent<IHealth> onHit;
        [SerializeField] private UnityEvent<IHealth> onHeal;
        [SerializeField] private UnityEvent<IHealth> onDied;

        private float maxHealth;

        private void Awake()
        {
            maxHealth = health;
        }

        private void Update()
        {
            if (healthOverTime != 0f)
            {
                float deltaHealth = healthOverTime * Time.deltaTime;

                if (deltaHealth < 0f)
                {
                    TakeDamage(deltaHealth);
                }
                else if (deltaHealth > 0f)
                {
                    Heal(deltaHealth);
                }
            }
        }

        public void Initialize(float value)
        {
            maxHealth = health = value;
        }

        public void TakeDamage(float amount)
        {
            if (health > 0f)
            {
                health -= amount;
                OnHit?.Invoke(this);
                
                if (health <= 0f)
                {
                    health = 0f;
                    OnDied?.Invoke(this);
                }
            }
        }

        public void Heal(float amount = -1f)
        {
            if (amount < 0f)
            {
                amount = maxHealth;
            }

            health += amount;
            health = Mathf.Min(health, maxHealth);
            OnHeal?.Invoke(this);
        }
    }
}