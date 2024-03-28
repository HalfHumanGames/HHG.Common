using UnityEngine;
using UnityEngine.Splines;

namespace HHG.Common.Runtime
{
    public static class SplineAnimateExtensions
    {
        public static Vector3 GetDirection(this SplineAnimate splineAnimate)
        {
            return splineAnimate.Container.EvaluateDirection(splineAnimate.NormalizedTime);
        }
    }
}