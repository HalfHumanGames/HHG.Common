using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class FixTransform : MonoBehaviour
    {
        [SerializeField] private Mode mode;
        [SerializeField] private Targets targets;

        private Vector3 position;
        private Quaternion rotation;

        public enum Mode
        {
            Local,
            Global
        }

        [Flags]
        public enum Targets
        {
            Position,
            Rotation,
        }

        private void Awake()
        {
            position = mode == Mode.Local ? transform.localPosition : transform.position;
            rotation = mode == Mode.Local ? transform.localRotation : transform.rotation;
        }

        private void LateUpdate()
        {
            if (mode == Mode.Local)
            {
                if (targets.HasFlag(Targets.Position)) transform.position = transform.parent.position + position;
                if (targets.HasFlag(Targets.Rotation)) transform.rotation = transform.parent.rotation * rotation;
            }
            else if (mode == Mode.Global)
            {
                if (targets.HasFlag(Targets.Position)) transform.position = position;
                if (targets.HasFlag(Targets.Rotation)) transform.rotation = rotation;
            }
        }
    }
}