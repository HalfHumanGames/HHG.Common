using System.Text;
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

        private static readonly (int Value, string Symbol)[] romanNumerals =
        {
            (1000, "M"),
            (900, "CM"),
            (500, "D"),
            (400, "CD"),
            (100, "C"),
            (90, "XC"),
            (50, "L"),
            (40, "XL"),
            (10, "X"),
            (9, "IX"),
            (5, "V"),
            (4, "IV"),
            (1, "I")
        };

        public static string ToRomanNumeral(this int number)
        {
            if (number <= 0) throw new System.ArgumentOutOfRangeException(nameof(number), "Roman numerals require a positive integer.");

            StringBuilder result = new StringBuilder();

            foreach ((int value, string symbol) in romanNumerals)
            {
                while (number >= value)
                {
                    result.Append(symbol);
                    number -= value;
                }
            }

            return result.ToString();
        }
    }
}