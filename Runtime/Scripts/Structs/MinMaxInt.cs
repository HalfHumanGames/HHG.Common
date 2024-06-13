using System;
using Random = UnityEngine.Random;

namespace HHG.Common.Runtime
{
    [Serializable]
    public struct MinMaxInt
    {
        public int NextRand => Random.Range(Min, Max + 1);

        public int Min;
        public int Max;

        public MinMaxInt(int val)
        {
            Min = Max = val;
        }

        public MinMaxInt(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public static implicit operator int(MinMaxInt val)
        {
            return val.NextRand;
        }

        public static implicit operator MinMaxInt(int val)
        {
            return new MinMaxInt(val);
        }
    }
}