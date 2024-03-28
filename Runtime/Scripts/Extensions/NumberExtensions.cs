using System;

namespace HHG.Common.Runtime
{
    public static class NumberExtensions
    {
        public static float RoundToNearest(this float number, float nearest)
        {
            if (nearest == 0)
            {
                return number;
            }
            float factor = 1 / nearest;
            float retval = number * factor;
            retval = (float)Math.Round(retval, MidpointRounding.AwayFromZero);
            return retval / factor;
        }
    }
}