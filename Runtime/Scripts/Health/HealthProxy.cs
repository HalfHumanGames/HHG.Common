using UnityEngine;
using UnityEngine.Events;

namespace HHG.Common.Runtime
{
    public class HealthProxy : MonoBehaviour, IHealth
    {
        public MonoBehaviour Mono => this;
        public float HealthValue => root.HealthValue;
        public float HealthPerc => root.HealthPerc;
        public UnityEvent<IHealth> OnHit => root.OnHit;
        public UnityEvent<IHealth> OnHeal => root.OnHeal;
        public UnityEvent<IHealth> OnDied => root.OnDied;
        public UnityEvent<IHealth> OnHealthUpdated => root.OnHealthUpdated;

        private IHealth _root;
        private IHealth root
        {
            get
            {
                if (_root == null)
                {
                    _root = transform.parent.GetComponentInParent<IHealth>();
                }
                return _root;
            }
        }

        public void Initialize(float health)
        {
            root.Initialize(health);
        }

        public void TakeDamage(float amount)
        {
            root.TakeDamage(amount);
        }

        public void Heal(float amount = -1f)
        {
            root.Heal(amount);
        }
    }
}