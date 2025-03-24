using UnityEngine;
using UnityEngine.Serialization;

namespace HHG.Common.Runtime
{
    public class MetaBehaviourRunner : MonoBehaviour
    {
        public MetaBehaviour Behaviour => behaviour;

        [SerializeReference, SubclassSelector, FormerlySerializedAs("_behaviour")] private MetaBehaviour behaviour;

        internal T SetBehaviour<T>(T metaBehaviour) where T : MetaBehaviour
        {
            behaviour = metaBehaviour;
            behaviour.SetRunner(this);
            return metaBehaviour;
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
            behaviour?.SetRunner(this);
            behaviour?.OnDrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            behaviour?.SetRunner(this);
            behaviour?.OnDrawGizmosSelected();
        }

        private void OnValidate()
        {
            behaviour?.SetRunner(this);
            behaviour?.OnValidate();
        }

        private void OnDestroy()
        {
            behaviour?.OnDestroy();
        }
    }
}