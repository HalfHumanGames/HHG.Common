using UnityEngine;
using UnityEngine.Serialization;

namespace HHG.Common.Runtime
{
    public class SmoothDamp : MonoBehaviour
    {
        [FormerlySerializedAs("target")] public Transform Target;
        [FormerlySerializedAs("smoothTime")] public float SmoothTime = 0.3f;
        [FormerlySerializedAs("maxSpeed")] public float MaxSpeed = 1000f;
        [FormerlySerializedAs("offset")] public Vector3 Offset;
        [System.NonSerialized] public Vector3 Velocity = Vector3.zero;

        private void Update()
        {
            if (Target != null)
            {
                transform.position = Vector3.SmoothDamp(transform.position, Target.TransformPoint(Offset), ref Velocity, SmoothTime, MaxSpeed);
            }
        }
    }
}