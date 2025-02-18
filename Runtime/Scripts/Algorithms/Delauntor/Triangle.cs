using System.Collections.Generic;

namespace HHG.Common.Runtime
{
    public partial class Delaunator
    {
        public struct Triangle
        {
            public int Index;
            public IEnumerable<Point> Points;

            public Triangle(int index, IEnumerable<Point> points)
            {
                Index = index;
                Points = points;
            }
        }
    }
}