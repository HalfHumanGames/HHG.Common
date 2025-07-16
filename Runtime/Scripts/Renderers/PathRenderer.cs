using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(LineRenderer))]
    public class PathRenderer : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 5f)] private float cornerRadius = 0.5f;
        [SerializeField, Range(2, 20)] private int cornerResolution = 5;
        [SerializeField] private GameObject headPrefab;
        [SerializeField] private GameObject tailPrefab;

        private LineRenderer lineRenderer;
        private GameObject head;
        private GameObject tail;
        private List<Vector3> points = new List<Vector3>();
        private List<Vector3> smoothedPoints = new List<Vector3>();
        private bool initialized;

        private void Awake()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (initialized) return;

            initialized = true;

            lineRenderer = GetComponent<LineRenderer>();
            head = headPrefab && !headPrefab.scene.IsValid() ? Instantiate(headPrefab, transform) : headPrefab;
            tail = tailPrefab && !tailPrefab.scene.IsValid() ? Instantiate(tailPrefab, transform) : tailPrefab;
        }

        public void SetPoints(IEnumerable<Vector3> newPoints)
        {
            EnsureInitialized();
            points.Clear();
            points.AddRange(newPoints);
            Render();
        }

        public void Clear()
        {
            EnsureInitialized();
            points.Clear();
            Render();
        }

        private void Render()
        {
            if (points.Count < 2)
            {
                lineRenderer.positionCount = 0;
                if (head) head.SetActive(false);
                if (tail) tail.SetActive(false);
                return;
            }

            smoothedPoints.Clear();

            for (int i = 0; i < points.Count; i++)
            {
                if (i == 0 || i == points.Count - 1)
                {
                    smoothedPoints.Add(points[i]);
                    continue;
                }

                Vector3 prev = points[i - 1];
                Vector3 current = points[i];
                Vector3 next = points[i + 1];

                Vector3 dirA = (current - prev).normalized;
                Vector3 dirB = (next - current).normalized;

                float dist = Mathf.Min(
                    cornerRadius,
                    Vector3.Distance(current, prev) * 0.5f,
                    Vector3.Distance(current, next) * 0.5f
                );

                Vector3 cornerStart = current - dirA * dist;
                Vector3 cornerEnd = current + dirB * dist;

                smoothedPoints.Add(cornerStart);

                for (int j = 1; j <= cornerResolution; j++)
                {
                    float t = j / (float)(cornerResolution + 1);
                    smoothedPoints.Add(GetCornerPoint(cornerStart, current, cornerEnd, t));
                }

                smoothedPoints.Add(cornerEnd);
            }

            smoothedPoints.Add(points[points.Count - 1]);
            RemoveClosePoints(smoothedPoints);

            lineRenderer.positionCount = smoothedPoints.Count;
            lineRenderer.SetPositions(smoothedPoints.ToArray());

            SetupCap(tail, smoothedPoints[0], smoothedPoints[1] - smoothedPoints[0]);
            SetupCap(head, smoothedPoints[^1], smoothedPoints[^1] - smoothedPoints[^2]);
        }

        private static Vector3 GetCornerPoint(Vector3 start, Vector3 corner, Vector3 end, float t)
        {
            Vector3 a = Vector3.Lerp(start, corner, t);
            Vector3 b = Vector3.Lerp(corner, end, t);
            return Vector3.Lerp(a, b, t);
        }

        private static void RemoveClosePoints(List<Vector3> inputPoints, float minDistance = 0.1f)
        {
            if (inputPoints.Count == 0) return;

            float sqrMinDist = minDistance * minDistance;
            int writeIndex = 1;

            for (int readIndex = 1; readIndex < inputPoints.Count; readIndex++)
            {
                if ((inputPoints[readIndex] - inputPoints[writeIndex - 1]).sqrMagnitude > sqrMinDist)
                {
                    inputPoints[writeIndex] = inputPoints[readIndex];
                    writeIndex++;
                }
            }

            if (writeIndex < inputPoints.Count)
            {
                inputPoints.RemoveRange(writeIndex, inputPoints.Count - writeIndex);
            }
        }


        private static void SetupCap(GameObject cap, Vector3 position, Vector3 direction)
        {
            direction.y = 0;
            cap.SetActive(true);
            cap.transform.position = position;
            Quaternion lookRot = Quaternion.LookRotation(direction.normalized, Vector3.up);
            Quaternion correction = Quaternion.Euler(90, -90, 0);
            cap.transform.rotation = lookRot * correction;
        }
    }
}