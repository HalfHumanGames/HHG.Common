using UnityEngine;

namespace HHG.Common.Runtime
{
    public class SyncTransform : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private bool syncPosition;
        [SerializeField] private bool syncRotation;
        [SerializeField] private bool syncLocalScale;

        private void LateUpdate()
        {
            if (syncPosition)
            {
                transform.position = target.position;
            }

            if (syncRotation)
            {
                transform.rotation = target.rotation;
            }

            if (syncLocalScale)
            {
                transform.localScale = target.localScale;
            }
        }
    }
}