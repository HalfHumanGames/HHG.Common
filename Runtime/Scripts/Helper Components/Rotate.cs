using UnityEngine;

namespace HHG.Common
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private Space space;
        [SerializeField] private TimeScale timeScale;

        private enum TimeScale
        {
            Scaled,
            Unscaled
        }

        private void Update()
        {
            float deltaTime = timeScale == TimeScale.Scaled ? Time.deltaTime : Time.unscaledDeltaTime;
            transform.Rotate(rotationAxis * (rotationSpeed * deltaTime), space);
        }
    }
}