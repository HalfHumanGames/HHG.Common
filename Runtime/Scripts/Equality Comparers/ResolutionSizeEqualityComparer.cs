using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class ResolutionSizeEqualityComparer : IEqualityComparer<Resolution>
    {
        public static readonly ResolutionSizeEqualityComparer Instance = new ResolutionSizeEqualityComparer();

        public bool Equals(Resolution a, Resolution b)
        {
            return a.width == b.width && a.height == b.height;
        }

        public int GetHashCode(Resolution a)
        {
            return a.width.GetHashCode() ^ (a.height.GetHashCode() << 2);
        }
    }
}