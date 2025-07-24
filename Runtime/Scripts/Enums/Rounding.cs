using UnityEngine;

namespace HHG.Common.Runtime
{
    public enum Rounding
    {
        None,
        Round,
        Ceil,
        Floor
    }

    public static class RoundingExtensions
    {
        public static float Round(this Rounding round, float f)
        {
            return round switch
            {
                Rounding.Round => Mathf.Round(f),
                Rounding.Ceil => Mathf.Ceil(f),
                Rounding.Floor => Mathf.Floor(f),
                _ => f
            };
        }
    }
}