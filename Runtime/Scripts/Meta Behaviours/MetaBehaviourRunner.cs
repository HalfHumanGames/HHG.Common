using UnityEngine;
using UnityEngine.Serialization;

namespace HHG.Common.Runtime
{
    public class MetaBehaviourRunner : MonoBehaviour
    {
        public MetaBehaviour Behaviour => behaviour;

        [SerializeReference, SerializeReferenceDropdown, FormerlySerializedAs("_behaviour")] private MetaBehaviour behaviour;

        internal MetaBehaviour SetBehaviour(MetaBehaviour metaBehaviour)
        {
            behaviour = metaBehaviour;
            behaviour.SetRunner(this);
            return behaviour;
        }

        private void Awake()
        {
            behaviour?.SetRunner(this);
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