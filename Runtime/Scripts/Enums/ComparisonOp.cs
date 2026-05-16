using UnityEngine;

namespace HHG.Common.Runtime
{
    public enum ComparisonOp
    {
        GreaterThan,
        GreaterThanEqualTo,
        LessThan,
        LessThanEqualTo,
        EqualTo,
        NotEqualTo
    }

    public static class ComparisonOpExtensions
    {
        public static bool Evaluate(this ComparisonOp op, float value, float threshold) => op switch
        {
            ComparisonOp.GreaterThan => value > threshold,
            ComparisonOp.GreaterThanEqualTo => value >= threshold,
            ComparisonOp.LessThan => value < threshold,
            ComparisonOp.LessThanEqualTo => value <= threshold,
            ComparisonOp.EqualTo => Mathf.Approximately(value, threshold),
            ComparisonOp.NotEqualTo => !Mathf.Approximately(value, threshold),
            _ => false
        };
    }
}