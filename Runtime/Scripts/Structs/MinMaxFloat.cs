using System;
using Random = UnityEngine.Random;

namespace HHG.Common.Runtime
{
    [Serializable]
    public struct MinMaxFloat
    {
        public float NextRand => Random.Range(Min, Max);

        public float Min;
        public float Max;

        public MinMaxFloat(float val)
        {
            Min = Max = val;
        }

        public MinMaxFloat(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public static implicit operator float(MinMaxFloat val)
        {
            return val.NextRand;
        }

        public static implicit operator MinMaxFloat(int val)
        {
            return new MinMaxFloat(val);
        }
    }
}