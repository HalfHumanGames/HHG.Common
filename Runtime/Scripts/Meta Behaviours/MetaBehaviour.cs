using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class MetaBehaviour : ICloneable<MetaBehaviour>
    {
        public bool enabled
        {
            get => runner != null && runner.enabled;
            set
            {
                if (runner != null)
                {
                    runner.enabled = value;
                }
            }
        }

        public MetaBehaviourRunner Runner => runner;
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;

        protected MetaBehaviourRunner runner;
        protected GameObject gameObject;
        protected Transform transform;

        internal MetaBehaviour SetRunner(MetaBehaviourRunner metaBehaviourRunner)
        {
            runner = metaBehaviourRunner;
            gameObject = metaBehaviourRunner.gameObject;
            transform = metaBehaviourRunner.transform;
            return this;
        }

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void FixedUpdate() { }
        public virtual void OnDestroy() { }
        public virtual void OnDrawGizmos() { }
        public virtual void OnDrawGizmosSelected() { }
        public virtual void OnValidate() { }

        public void Destroy(float delay = 0f) => Object.Destroy(runner, delay);
        public void SmartDestroy() => ObjectUtil.SmartDestroy(runner);

        public MetaBehaviour Clone()
        {
            return (MetaBehaviour)JsonUtility.FromJson(JsonUtility.ToJson(this), GetType());
        }
    }
}