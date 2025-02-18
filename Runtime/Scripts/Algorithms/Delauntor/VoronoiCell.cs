namespace HHG.Common.Runtime
{
    public partial class Delaunator
    {
        public struct VoronoiCell
        {
            public int Index;
            public Point[] Points;

            public VoronoiCell(int index, Point[] points)
            {
                Index = index;
                Points = points;
            }
        }
    }
}