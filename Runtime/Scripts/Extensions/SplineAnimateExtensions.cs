using UnityEngine;
using UnityEngine.Splines;

namespace HHG.Common.Runtime
{
    public static class SplineAnimateExtensions
    {
        public static Vector3 GetDirection(this SplineAnimate splineAnimate, SplinePath<Spline> path)
        {
            return splineAnimate.Container.EvaluateDirection(path, splineAnimate.NormalizedTime);
        }
    }
}