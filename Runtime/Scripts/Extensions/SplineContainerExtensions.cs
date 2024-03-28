using UnityEngine;
using UnityEngine.Splines;

namespace HHG.Common
{
    public static class SplineContainerExtensions
    {
        public static Vector3 EvaluateDirection(this SplineContainer splineContainer, float normalizedTime)
        {
            return Vector3.Normalize(splineContainer.EvaluateTangent(normalizedTime));
        }
    }
}