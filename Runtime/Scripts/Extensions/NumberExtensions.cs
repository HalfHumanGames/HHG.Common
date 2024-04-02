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

        public static bool HasFlag(this int flags, int flag)
        {
            return (flags & flag) == flag;
        }

        public static int WithFlag(this int flags, int flag, bool on = true)
        {
            return flags | flag;
        }

        public static int WithoutFlag(this int flags, int flag)
        {
            return flags & ~flag;
        }

        public static int WithFlagInverse(this int flags, int flag)
        {
            return flags.HasFlag(flag) ? flags.WithoutFlag(flag) : flags.WithFlag(flag);
        }

        public static int ToFlags(this int[] ints)
        {
            int flags = 0;
            Array.ForEach(ints, i => flags |= 1 << i);
            return flags;
        }
    }
}