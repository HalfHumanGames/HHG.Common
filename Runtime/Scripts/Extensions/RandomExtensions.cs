using UnityEngine;

namespace HHG.Common
{
    public static class RandomExtensions
    {
        public static Vector3 InsideCircle(float min, float max)
        {
            return Random.insideUnitCircle.normalized * Random.Range(min, max);
        }

        public static Vector3 InsideSphere(float min, float max)
        {
            return Random.insideUnitSphere.normalized * Random.Range(min, max);
        }

        public static Vector3 InsideCone(Vector3 direction, Vector3 size, float min, float max)
        {
            float distance = Random.Range(min, max);
            float deltaX = size.x / 2f;
            float deltaY = size.y / 2f;
            float deltaZ = size.z / 2f;
            float rotationX = Random.Range(-deltaX, deltaX);
            float rotationY = Random.Range(-deltaY, deltaY);
            float rotationZ = Random.Range(-deltaZ, deltaZ);
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
            return (rotation * direction).normalized * distance;
        }
    }
}