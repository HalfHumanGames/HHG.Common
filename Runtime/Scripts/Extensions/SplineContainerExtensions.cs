using UnityEngine;
using UnityEngine.Splines;

namespace HHG.Common.Runtime
{
    public static class SplineContainerExtensions
    {
        public static Vector3 EvaluateDirection(this SplineContainer splineContainer, SplinePath<Spline> path, float normalizedTime)
        {
            return Vector3.Normalize(splineContainer.EvaluateTangent(path, normalizedTime));
        }
    }
}