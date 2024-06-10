using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class MetaBehaviour : ICloneable<MetaBehaviour>
    {
        public bool IsEnabled
        {
            get => runner.enabled; 
            set => runner.enabled = value;
        }

        [SerializeField, HideInInspector] protected MetaBehaviourRunner runner;
        [SerializeField, HideInInspector] protected GameObject gameObject;
        [SerializeField, HideInInspector] protected Transform transform;

        internal MetaBehaviour EnsureAttached(MetaBehaviourRunner metaBehaviourRunner)
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

        public MetaBehaviour Clone()
        {
            return (MetaBehaviour)JsonUtility.FromJson(JsonUtility.ToJson(this), GetType());
        }
    }
}