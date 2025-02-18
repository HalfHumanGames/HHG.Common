namespace HHG.Common.Runtime
{
    public partial class Delaunator
    {
        public struct Edge
        {
            public int Index;
            public Point P;
            public Point Q;

            public Edge(int e, Point p, Point q)
            {
                Index = e;
                P = p;
                Q = q;
            }
        }
    }
}