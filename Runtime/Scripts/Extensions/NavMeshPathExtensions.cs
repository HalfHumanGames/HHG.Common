using UnityEngine;
using UnityEngine.AI;

namespace HHG.Common.Runtime
{
    public static class NavMeshPathExtensions
    {
        public static float GetDistance(this NavMeshPath path)
        {
            float length = 0f;

            if (path.corners.Length > 1)
            {
                for (int i = 1; i < path.corners.Length; i++)
                {
                    length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
            }

            return length;
        }

        public static Vector3 EvaluatePosition(this NavMeshPath path, float distance)
        {
            Vector3[] corners = path.corners;

            float totalDistance = 0f;
            int len = corners.Length;

            for (int i = 0; i < len - 1; i++)
            {
                float segmentDistance = Vector3.Distance(corners[i], corners[i + 1]);

                if (totalDistance + segmentDistance > distance)
                {
                    float distanceAlongSegment = distance - totalDistance;
                    float perc = distanceAlongSegment / segmentDistance;
                    return Vector3.Lerp(corners[i], corners[i + 1], perc);
                }
                if (totalDistance + segmentDistance == distance)
                {
                    return corners[i + 1];
                }
                else
                {
                    totalDistance += segmentDistance;
                }
            }

            return corners.Length > 0 ? corners[len - 1] : default;
        }

        public static Vector3 EvaluateDirection(this NavMeshPath path, float distance)
        {
            Vector3[] corners = path.corners;

            float totalDistance = 0f;
            int len = corners.Length;

            for (int i = 0; i < len - 1; i++)
            {
                float segmentDistance = Vector3.Distance(corners[i], corners[i + 1]);

                if (totalDistance + segmentDistance >= distance)
                {
                    return corners[i + 1] - corners[i];
                }
                else
                {
                    totalDistance += segmentDistance;
                }
            }

            return corners[len - 1] - corners[len - 2];
        }
    }
}