namespace HHG.Common.Runtime
{
    public partial class Delaunator
    {
        public struct Point
        {
            public float X { get; set; }
            public float Y { get; set; }

            public Point(float x, float y)
            {
                X = x;
                Y = y;
            }

            public override string ToString() => $"Point: ({X},{Y})";
        }
    }
}