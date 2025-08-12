using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class NumberExtensions
    {
        public static float Round(this float number)
        {
            return Mathf.Round(number);
        }

        public static int RoundToInt(this float number)
        {
            return Mathf.RoundToInt(number);
        }

        public static float RoundToNearest(this float number, float nearest)
        {
            if (nearest == 0)
            {
                return number;
            }
            float factor = 1 / nearest;
            float retval = number * factor;
            retval = (float)System.Math.Round(retval, System.MidpointRounding.AwayFromZero);
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
            System.Array.ForEach(ints, i => flags |= 1 << i);
            return flags;
        }
    }
}