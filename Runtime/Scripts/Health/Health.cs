using UnityEngine;
using UnityEngine.Events;

namespace HHG.Common.Runtime
{
    public class Health : MonoBehaviour, IHealth
    {
        private const float dieThreshold = .01f;

        public MonoBehaviour Mono => this;
        public float HealthValue => health;
        public float HealthPerc => maxHealth == 0f ? 1f : health / maxHealth;
        public float PreviousHealthPerc => maxHealth == 0f ? 1f : previousHealthValue / maxHealth;
        public float PreviousHealthValue => previousHealthValue;
        public float PreviousHealthValueDelta => previousHealthValueDelta;
        public UnityEvent<IHealth> OnHit => onHit;
        public UnityEvent<IHealth> OnHeal => onHeal;
        public UnityEvent<IHealth> OnDied => onDied;
        public UnityEvent<IHealth> OnHealthUpdated => onHealthUpdated;

        private float health
        {
            get => _health;
            set
            {
                if (_health != value)
                {
                    previousHealthValue = _health;
                    _health = value;
                    onHealthUpdated?.Invoke(this);
                }
            }
        }

        [SerializeField] private float _health;
        [SerializeField] private float healthOverTime;
        [SerializeField] private UnityEvent<IHealth> onHit = new UnityEvent<IHealth>();
        [SerializeField] private UnityEvent<IHealth> onHeal = new UnityEvent<IHealth>();
        [SerializeField] private UnityEvent<IHealth> onDied = new UnityEvent<IHealth>();
        [SerializeField] private UnityEvent<IHealth> onHealthUpdated = new UnityEvent<IHealth>();

        private float maxHealth;
        private float previousHealthValue;
        private float previousHealthValueDelta => health - previousHealthValue;

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
                    TakeDamage(Mathf.Abs(deltaHealth));
                }
                else if (deltaHealth > 0f)
                {
                    Heal(deltaHealth);
                }
            }
        }

        private void CheckForDeath()
        {
            if (health <= dieThreshold)
            {
                health = 0f;
                OnDied?.Invoke(this);
            }
        }

        public void Initialize(float value)
        {
            maxHealth = health = value;
            CheckForDeath();
        }

        public void Initialize(float value, float overTime)
        {
            maxHealth = health = value;
            healthOverTime = overTime;
            CheckForDeath();
        }

        public void InitializeValue(float value)
        {
            health = value;
            CheckForDeath();
        }

        public void TakeDamage(float amount)
        {
            if (health > 0f)
            {
                health -= amount;
                OnHit?.Invoke(this);
                CheckForDeath();
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