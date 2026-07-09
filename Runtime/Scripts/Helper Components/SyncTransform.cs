using UnityEngine;
using UnityEngine.Serialization;

namespace HHG.Common.Runtime
{
    public class SyncTransform : MonoBehaviour
    {
        // Public so can modify at runtime without setter methods
        [FormerlySerializedAs("target")] public Transform Target;
        [FormerlySerializedAs("syncPosition")] public bool SyncPosition;
        [FormerlySerializedAs("syncRotation")] public bool SyncRotation;
        [FormerlySerializedAs("syncLocalScale")] public bool SyncLocalScale;

        private void LateUpdate()
        {
            if (Target == null) return;
            if (SyncPosition) transform.position = Target.position;
            if (SyncRotation) transform.rotation = Target.rotation;
            if (SyncLocalScale) transform.localScale = Target.localScale;
        }
    }
}