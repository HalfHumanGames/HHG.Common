using UnityEngine;
using UnityEngine.Serialization;

namespace HHG.Common.Runtime
{
    public class SyncTransform : MonoBehaviour
    {
        // Public so can modify at runtime without setter methods
        [FormerlySerializedAs("target")] public Transform Target;

        [FormerlySerializedAs("syncPosition")] public bool SyncPosition;
        [ShowIf(nameof(SyncPosition), true)] public float PositionSmoothTime;
        [ShowIf(nameof(SyncPosition), true)] public float PositionMaxSpeed = 1000f;
        [ShowIf(nameof(SyncPosition), true)] public Vector3 PositionOffset;

        [FormerlySerializedAs("syncRotation")] public bool SyncRotation;
        [ShowIf(nameof(SyncRotation), true)] public float RotationSmoothTime;
        [ShowIf(nameof(SyncRotation), true)] public float RotationMaxSpeed = 1000f;
        [ShowIf(nameof(SyncRotation), true)] public Vector3 RotationOffset;

        [FormerlySerializedAs("syncLocalScale")] public bool SyncLocalScale;
        [ShowIf(nameof(SyncLocalScale), true)] public float ScaleSmoothTime;
        [ShowIf(nameof(SyncLocalScale), true)] public float ScaleMaxSpeed = 1000f;
        [ShowIf(nameof(SyncLocalScale), true)] public Vector3 ScaleOffset;

        private Vector3 positionVelocity;
        private Quaternion rotationVelocity;
        private Vector3 scaleVelocity;

        private void LateUpdate()
        {
            if (Target == null)
            {
                return;
            }

            if (SyncPosition)
            {
                Vector3 targetPosition = Target.TransformPoint(PositionOffset);

                if (PositionSmoothTime <= 0f)
                {
                    transform.position = targetPosition;
                }
                else
                {
                    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref positionVelocity, PositionSmoothTime, PositionMaxSpeed);
                }
            }

            if (SyncRotation)
            {
                Quaternion targetRotation = Target.rotation * Quaternion.Euler(RotationOffset);

                if (RotationSmoothTime <= 0f)
                {
                    transform.rotation = targetRotation;
                }
                else
                {
                    transform.rotation = MathUtil.SmoothDamp(transform.rotation, targetRotation, ref rotationVelocity, RotationSmoothTime, RotationMaxSpeed);
                }
            }

            if (SyncLocalScale)
            {
                Vector3 targetScale = Target.localScale + ScaleOffset;

                if (ScaleSmoothTime <= 0f)
                {
                    transform.localScale = targetScale;
                }
                else
                {
                    transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref scaleVelocity, ScaleSmoothTime, ScaleMaxSpeed);
                }
            }
        }
    }
}
