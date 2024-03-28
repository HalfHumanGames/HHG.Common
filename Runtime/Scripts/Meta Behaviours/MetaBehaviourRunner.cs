using UnityEngine;

namespace HHG.Common
{
    public class MetaBehaviourRunner : MonoBehaviour
    {
        public MetaBehaviour Behaviour => behaviour;

        [SerializeReference] private MetaBehaviour behaviour;

        internal MetaBehaviour AttachBehaviour(MetaBehaviour metaBehaviour)
        {
            behaviour = metaBehaviour;
            behaviour.AttachToRunner(this);
            return behaviour;
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

        private void OnDestroy()
        {
            behaviour?.OnDestroy();
        }
    }
}