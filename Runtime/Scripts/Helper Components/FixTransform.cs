using UnityEngine;

namespace HHG.Common.Runtime
{
    public class FixTransform : MonoBehaviour
    {
        [SerializeField] private Mode fixPosition;
        [SerializeField] private Mode fixRotation;
        [SerializeField] private Mode fixScale;
        [SerializeField, ShowIf(nameof(useGlobalScale), true)] private Vector3 globalScale;

        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;

        private bool useGlobalScale => fixScale == Mode.Global;

        public enum Mode
        {
            None,
            Local,
            Global
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

            if (fixScale == Mode.Local) transform.SetGlobalScale(Vector3.Scale(transform.parent.lossyScale, scale));
            else if (fixScale == Mode.Global) transform.SetGlobalScale(Vector3.Scale(transform.parent.lossyScale.Signed(), scale));
        }

        public void Initialize(Mode fixPosition = Mode.None, Mode fixRotation = Mode.None, Mode fixScale = Mode.None, Vector3 globalScale = default)
        {
            this.fixPosition = fixPosition;
            this.fixRotation = fixRotation;
            this.fixScale = fixScale;
            Initialize();
        }

        [ContextMenu("Initialize")]
        public void Initialize()
        {
            position = fixPosition == Mode.Local ? transform.localPosition : transform.position;
            rotation = fixRotation == Mode.Local ? transform.localRotation : transform.rotation;
            scale = fixScale == Mode.Local ? transform.localScale : globalScale;
        }
    }
}