using UnityEngine;

namespace HHG.Common.Runtime
{
    public class MetaBehaviourRunner : MonoBehaviour
    {
        public MetaBehaviour Behaviour => behaviour;

        [SerializeReference, SerializeReferenceDropdown] private MetaBehaviour _behaviour;

        protected MetaBehaviour behaviour => _behaviour?.EnsureAttached(this);

        internal MetaBehaviour AttachBehaviour(MetaBehaviour metaBehaviour)
        {
            _behaviour = metaBehaviour;
            return behaviour; // Use getter to ensure attached
        }

        private void Awake()
        {
            behaviour?.Awake();
        }

        private void Start()
        {
            behaviour?.Start();
        }

        private void OnEnable()
        {
            behaviour?.OnEnable();
        }

        private void OnDisable()
        {
            behaviour?.OnDisable();
        }

        private void Update()
        {
            behaviour?.Update();
        }

        private void LateUpdate()
        {
            behaviour?.LateUpdate();
        }

        private void FixedUpdate()
        {
            behaviour?.FixedUpdate();
        }

        private void OnDrawGizmos()
        {
            behaviour?.OnDrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            behaviour?.OnDrawGizmosSelected();
        }

        private void OnValidate()
        {
            behaviour?.OnValidate();
        }

        private void OnDestroy()
        {
            behaviour?.OnDestroy();
        }
    }
}