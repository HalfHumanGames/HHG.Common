using UnityEngine;

namespace HHG.Common.Runtime
{
    public class SmoothDamp : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime = 0.3f;
        [SerializeField] private float maxSpeed = 1000f;
        [SerializeField] private Vector3 offset;

        private Vector3 velocity = Vector3.zero;

        private void Update()
        {
            if (target != null)
            {
                transform.position = Vector3.SmoothDamp(transform.position, target.TransformPoint(offset), ref velocity, smoothTime, maxSpeed);
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}