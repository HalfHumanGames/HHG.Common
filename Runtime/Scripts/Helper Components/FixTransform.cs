using UnityEngine;
using UnityEngine.Serialization;

namespace HHG.Common.Runtime
{
    public class FixTransform : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("mode")] private Mode fixPosition;
        [SerializeField] private Mode fixRotation;

        private Vector3 position;
        private Quaternion rotation;

        public enum Mode
        {
            Local,
            Global,
            None
        }

        private void Awake()
        {
            Initialize();
        }

        private void LateUpdate()
        {
            if (fixPosition == Mode.Local) transform.position = transform.parent.position + position;
            else if (fixPosition == Mode.Global) transform.position = position;

            if (fixRotation == Mode.Local) transform.rotation = transform.parent.rotation * rotation;
            else if (fixRotation == Mode.Global) transform.rotation = rotation;
        }

        public void Initialize(Mode fixPosition, Mode fixRotation)
        {
            this.fixPosition = fixPosition;
            this.fixRotation = fixRotation;
            Initialize();
        }

        [ContextMenu("Initialize")]
        public void Initialize()
        {
            position = fixPosition == Mode.Local ? transform.localPosition : transform.position;
            rotation = fixRotation == Mode.Local ? transform.localRotation : transform.rotation;
        }
    }
}