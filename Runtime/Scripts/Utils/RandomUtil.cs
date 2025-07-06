using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class RandomUtil
    {
        public static bool Chance(float chance)
        {
            return Random.value < chance;
        }

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

        public static Vector3 InsideBounds(Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public static Vector3Int InsideBounds(BoundsInt bounds)
        {
            return new Vector3Int(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }
    }
}