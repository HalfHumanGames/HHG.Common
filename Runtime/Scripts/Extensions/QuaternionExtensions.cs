using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class QuaternionExtensions
    {
        public static Quaternion RotateTowards2D(Quaternion rotation, Vector2 direction, Vector2 forward, float maxDegreesDelta)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - Vector3.SignedAngle(Vector3.right, forward, Vector3.forward);
            return Quaternion.RotateTowards(rotation, Quaternion.Euler(0, 0, angle), maxDegreesDelta);
        }
    }
}