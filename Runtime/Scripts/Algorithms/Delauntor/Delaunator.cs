using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public partial class Delaunator
    {
        private static readonly float epsilon = Mathf.Pow(2, -52);

        public int[] Triangles { get; private set; }
        public int[] HalfEdges { get; private set; }
        public Point[] Points { get; private set; }
        public int[] Hull { get; private set; }

        private readonly int hashSize;
        private readonly int[] hullPrev;
        private readonly int[] hullNext;
        private readonly int[] hullTri;
        private readonly int[] hullHash;

        private float cx;
        private float cy;

        private int trianglesLen;
        private readonly float[] coords;
        private readonly int hullStart;
        private readonly int hullSize;
        private readonly int[] edgeStack = new int[512];

        public Delaunator(Point[] points)
        {
            if (points.Length < 3)
            {
                throw new ArgumentOutOfRangeException("Need at least 3 points");
            }

            Points = points;
            coords = new float[Points.Length * 2];

            for (int i = 0; i < Points.Length; i++)
            {
                Point p = Points[i];
                coords[2 * i] = p.X;
                coords[2 * i + 1] = p.Y;
            }

            int n = points.Length;
            int maxTriangles = 2 * n - 5;

            Triangles = new int[maxTriangles * 3];
            HalfEdges = new int[maxTriangles * 3];
            hashSize = (int)Math.Ceiling(Math.Sqrt(n));

            hullPrev = new int[n];
            hullNext = new int[n];
            hullTri = new int[n];
            hullHash = new int[hashSize];

            int[] ids = new int[n];

            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;

            for (int i = 0; i < n; i++)
            {
                float x = coords[2 * i];
                float y = coords[2 * i + 1];

                if (x < minX) { minX = x; }
                if (y < minY) { minY = y; }
                if (x > maxX) { maxX = x; }
                if (y > maxY) { maxY = y; }

                ids[i] = i;
            }

            float cx = (minX + maxX) / 2;
            float cy = (minY + maxY) / 2;

            float minDist = float.PositiveInfinity;
            int i0 = 0;
            int i1 = 0;
            int i2 = 0;

            for (int i = 0; i < n; i++)
            {
                float d = Dist(cx, cy, coords[2 * i], coords[2 * i + 1]);

                if (d < minDist)
                {
                    i0 = i;
                    minDist = d;
                }
            }
            float i0x = coords[2 * i0];
            float i0y = coords[2 * i0 + 1];

            minDist = float.PositiveInfinity;

            for (int i = 0; i < n; i++)
            {
                if (i == i0)
                {
                    continue;
                }

                float d = Dist(i0x, i0y, coords[2 * i], coords[2 * i + 1]);

                if (d < minDist && d > 0)
                {
                    i1 = i;
                    minDist = d;
                }
            }

            float i1x = coords[2 * i1];
            float i1y = coords[2 * i1 + 1];

            float minRadius = float.PositiveInfinity;

            for (int i = 0; i < n; i++)
            {
                if (i == i0 || i == i1)
                {
                    continue;
                }

                float r = Circumradius(i0x, i0y, i1x, i1y, coords[2 * i], coords[2 * i + 1]);

                if (r < minRadius)
                {
                    i2 = i;
                    minRadius = r;
                }
            }
            float i2x = coords[2 * i2];
            float i2y = coords[2 * i2 + 1];

            if (minRadius == float.PositiveInfinity)
            {
                throw new Exception("No Delaunay triangulation exists for this input.");
            }

            if (Orient(i0x, i0y, i1x, i1y, i2x, i2y))
            {
                int i = i1;
                float x = i1x;
                float y = i1y;
                i1 = i2;
                i1x = i2x;
                i1y = i2y;
                i2 = i;
                i2x = x;
                i2y = y;
            }

            Point center = Circumcenter(i0x, i0y, i1x, i1y, i2x, i2y);
            this.cx = center.X;
            this.cy = center.Y;

            float[] dists = new float[n];
            for (int i = 0; i < n; i++)
            {
                dists[i] = Dist(coords[2 * i], coords[2 * i + 1], center.X, center.Y);
            }

            Quicksort(ids, dists, 0, n - 1);

            hullStart = i0;
            hullSize = 3;

            hullNext[i0] = hullPrev[i2] = i1;
            hullNext[i1] = hullPrev[i0] = i2;
            hullNext[i2] = hullPrev[i1] = i0;

            hullTri[i0] = 0;
            hullTri[i1] = 1;
            hullTri[i2] = 2;

            hullHash[HashKey(i0x, i0y)] = i0;
            hullHash[HashKey(i1x, i1y)] = i1;
            hullHash[HashKey(i2x, i2y)] = i2;

            trianglesLen = 0;
            AddTriangle(i0, i1, i2, -1, -1, -1);

            float xp = 0;
            float yp = 0;

            for (int k = 0; k < ids.Length; k++)
            {
                int i = ids[k];
                float x = coords[2 * i];
                float y = coords[2 * i + 1];

                if (k > 0 && Math.Abs(x - xp) <= epsilon && Math.Abs(y - yp) <= epsilon)
                {
                    continue;
                }

                xp = x;
                yp = y;

                if (i == i0 || i == i1 || i == i2)
                {
                    continue;
                }

                int start = 0;
                for (int j = 0; j < hashSize; j++)
                {
                    int key = HashKey(x, y);
                    start = hullHash[(key + j) % hashSize];
                    if (start != -1 && start != hullNext[start])
                    {
                        break;
                    }
                }

                start = hullPrev[start];
                int e = start;
                int q = hullNext[e];

                while (!Orient(x, y, coords[2 * e], coords[2 * e + 1], coords[2 * q], coords[2 * q + 1]))
                {
                    e = q;
                    if (e == start)
                    {
                        e = int.MaxValue;
                        break;
                    }

                    q = hullNext[e];
                }

                if (e == int.MaxValue)
                {
                    continue;
                }

                int t = AddTriangle(e, i, hullNext[e], -1, -1, hullTri[e]);

                hullTri[i] = Legalize(t + 2);
                hullTri[e] = t;
                hullSize++;

                int next = hullNext[e];
                q = hullNext[next];

                while (Orient(x, y, coords[2 * next], coords[2 * next + 1], coords[2 * q], coords[2 * q + 1]))
                {
                    t = AddTriangle(next, i, q, hullTri[i], -1, hullTri[next]);
                    hullTri[i] = Legalize(t + 2);
                    hullNext[next] = next;
                    hullSize--;
                    next = q;

                    q = hullNext[next];
                }

                if (e == start)
                {
                    q = hullPrev[e];

                    while (Orient(x, y, coords[2 * q], coords[2 * q + 1], coords[2 * e], coords[2 * e + 1]))
                    {
                        t = AddTriangle(q, i, e, -1, hullTri[e], hullTri[q]);
                        Legalize(t + 2);
                        hullTri[q] = t;
                        hullNext[e] = e;
                        hullSize--;
                        e = q;

                        q = hullPrev[e];
                    }
                }

                hullStart = hullPrev[i] = e;
                hullNext[e] = hullPrev[next] = i;
                hullNext[i] = next;

                hullHash[HashKey(x, y)] = i;
                hullHash[HashKey(coords[2 * e], coords[2 * e + 1])] = e;
            }

            Hull = new int[hullSize];
            int s = hullStart;
            for (int i = 0; i < hullSize; i++)
            {
                Hull[i] = s;
                s = hullNext[s];
            }

            hullPrev = hullNext = hullTri = null;

            Triangles = Triangles.Take(trianglesLen).ToArray();
            HalfEdges = HalfEdges.Take(trianglesLen).ToArray();
        }

        private int Legalize(int a)
        {
            int i = 0;
            int ar;

            while (true)
            {
                int b = HalfEdges[a];

                int a0 = a - a % 3;
                ar = a0 + (a + 2) % 3;

                if (b == -1)
                {
                    if (i == 0)
                    {
                        break;
                    }

                    a = edgeStack[--i];
                    continue;
                }

                int b0 = b - b % 3;
                int al = a0 + (a + 1) % 3;
                int bl = b0 + (b + 2) % 3;

                int p0 = Triangles[ar];
                int pr = Triangles[a];
                int pl = Triangles[al];
                int p1 = Triangles[bl];

                bool illegal = InCircle(
                    coords[2 * p0], coords[2 * p0 + 1],
                    coords[2 * pr], coords[2 * pr + 1],
                    coords[2 * pl], coords[2 * pl + 1],
                    coords[2 * p1], coords[2 * p1 + 1]);

                if (illegal)
                {
                    Triangles[a] = p1;
                    Triangles[b] = p0;

                    int hbl = HalfEdges[bl];

                    if (hbl == -1)
                    {
                        int e = hullStart;
                        do
                        {
                            if (hullTri[e] == bl)
                            {
                                hullTri[e] = a;
                                break;
                            }
                            e = hullPrev[e];
                        } while (e != hullStart);
                    }
                    Link(a, hbl);
                    Link(b, HalfEdges[ar]);
                    Link(ar, bl);

                    int br = b0 + (b + 1) % 3;

                    if (i < edgeStack.Length)
                    {
                        edgeStack[i++] = br;
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        break;
                    }

                    a = edgeStack[--i];
                }
            }

            return ar;
        }

        private static bool InCircle(float ax, float ay, float bx, float by, float cx, float cy, float px, float py)
        {
            float dx = ax - px;
            float dy = ay - py;
            float ex = bx - px;
            float ey = by - py;
            float fx = cx - px;
            float fy = cy - py;

            float ap = dx * dx + dy * dy;
            float bp = ex * ex + ey * ey;
            float cp = fx * fx + fy * fy;

            return dx * (ey * cp - bp * fy) -
                   dy * (ex * cp - bp * fx) +
                   ap * (ex * fy - ey * fx) < 0;
        }

        private int AddTriangle(int i0, int i1, int i2, int a, int b, int c)
        {
            int t = trianglesLen;

            Triangles[t] = i0;
            Triangles[t + 1] = i1;
            Triangles[t + 2] = i2;

            Link(t, a);
            Link(t + 1, b);
            Link(t + 2, c);

            trianglesLen += 3;
            return t;
        }

        private void Link(int a, int b)
        {
            HalfEdges[a] = b;
            if (b != -1)
            {
                HalfEdges[b] = a;
            }
        }

        private int HashKey(float x, float y) => (int)(Math.Floor(PseudoAngle(x - cx, y - cy) * hashSize) % hashSize);

        private static float PseudoAngle(float dx, float dy)
        {
            float p = dx / (Math.Abs(dx) + Math.Abs(dy));
            return (dy > 0 ? 3 - p : 1 + p) / 4;
        }

        private static void Quicksort(int[] ids, float[] dists, int left, int right)
        {
            if (right - left <= 20)
            {
                for (int i = left + 1; i <= right; i++)
                {
                    int temp = ids[i];
                    float tempDist = dists[temp];
                    int j = i - 1;

                    while (j >= left && dists[ids[j]] > tempDist)
                    {
                        ids[j + 1] = ids[j--];
                    }

                    ids[j + 1] = temp;
                }
            }
            else
            {
                int median = (left + right) >> 1;
                int i = left + 1;
                int j = right;

                Swap(ids, median, i);

                if (dists[ids[left]] > dists[ids[right]]) { Swap(ids, left, right); }
                if (dists[ids[i]] > dists[ids[right]]) { Swap(ids, i, right); }
                if (dists[ids[left]] > dists[ids[i]]) { Swap(ids, left, i); }

                int temp = ids[i];
                float tempDist = dists[temp];

                while (true)
                {
                    do { i++; } while (dists[ids[i]] < tempDist);
                    do { j--; } while (dists[ids[j]] > tempDist);

                    if (j < i)
                    {
                        break;
                    }

                    Swap(ids, i, j);
                }

                ids[left + 1] = ids[j];
                ids[j] = temp;

                if (right - i + 1 >= j - left)
                {
                    Quicksort(ids, dists, i, right);
                    Quicksort(ids, dists, left, j - 1);
                }
                else
                {
                    Quicksort(ids, dists, left, j - 1);
                    Quicksort(ids, dists, i, right);
                }
            }
        }

        private static void Swap(int[] arr, int i, int j)
        {
            int tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }

        private static bool Orient(float px, float py, float qx, float qy, float rx, float ry) => (qy - py) * (rx - qx) - (qx - px) * (ry - qy) < 0;

        private static float Circumradius(float ax, float ay, float bx, float by, float cx, float cy)
        {
            float dx = bx - ax;
            float dy = by - ay;
            float ex = cx - ax;
            float ey = cy - ay;
            float bl = dx * dx + dy * dy;
            float cl = ex * ex + ey * ey;
            float d = 0.5f / (dx * ey - dy * ex);
            float x = (ey * bl - dy * cl) * d;
            float y = (dx * cl - ex * bl) * d;
            return x * x + y * y;
        }

        private static Point Circumcenter(float ax, float ay, float bx, float by, float cx, float cy)
        {
            float dx = bx - ax;
            float dy = by - ay;
            float ex = cx - ax;
            float ey = cy - ay;
            float bl = dx * dx + dy * dy;
            float cl = ex * ex + ey * ey;
            float d = .5f / (dx * ey - dy * ex);
            float x = ax + (ey * bl - dy * cl) * d;
            float y = ay + (dx * cl - ex * bl) * d;
            return new Point(x, y);
        }

        private static float Dist(float ax, float ay, float bx, float by)
        {
            float dx = ax - bx;
            float dy = ay - by;
            return dx * dx + dy * dy;
        }

        public IEnumerable<Triangle> GetTriangles()
        {
            for (int t = 0; t < Triangles.Length / 3; t++)
            {
                yield return new Triangle(t, GetTrianglePoints(t));
            }
        }

        public IEnumerable<Edge> GetEdges()
        {
            for (int e = 0; e < Triangles.Length; e++)
            {
                if (e > HalfEdges[e])
                {
                    Point p = Points[Triangles[e]];
                    Point q = Points[Triangles[NextHalfedge(e)]];
                    yield return new Edge(e, p, q);
                }
            }
        }

        public IEnumerable<Edge> GetVoronoiEdges(Func<int, Point> triangleVerticeSelector = null)
        {
            if (triangleVerticeSelector == null)
            {
                triangleVerticeSelector = x => GetCentroid(x);
            }

            for (int e = 0; e < Triangles.Length; e++)
            {
                if (e < HalfEdges[e])
                {
                    Point p = triangleVerticeSelector(TriangleOfEdge(e));
                    Point q = triangleVerticeSelector(TriangleOfEdge(HalfEdges[e]));
                    yield return new Edge(e, p, q);
                }
            }
        }

        public IEnumerable<Edge> GetVoronoiEdgesBasedOnCircumCenter() => GetVoronoiEdges(GetTriangleCircumcenter);
        public IEnumerable<Edge> GetVoronoiEdgesBasedOnCentroids() => GetVoronoiEdges(GetCentroid);

        public IEnumerable<VoronoiCell> GetVoronoiCells(Func<int, Point> triangleVerticeSelector = null)
        {
            if (triangleVerticeSelector == null)
            {
                triangleVerticeSelector = x => GetCentroid(x);
            }

            HashSet<int> seen = new HashSet<int>();
            List<Point> vertices = new List<Point>(10);

            for (int e = 0; e < Triangles.Length; e++)
            {
                int pointIndex = Triangles[NextHalfedge(e)];
                if (seen.Add(pointIndex))
                {
                    foreach (int edge in EdgesAroundPoint(e))
                    {
                        vertices.Add(triangleVerticeSelector.Invoke(TriangleOfEdge(edge)));
                    }
                    yield return new VoronoiCell(pointIndex, vertices.ToArray());
                    vertices.Clear();
                }
            }
        }

        public IEnumerable<VoronoiCell> GetVoronoiCellsBasedOnCircumcenters() => GetVoronoiCells(GetTriangleCircumcenter);
        public IEnumerable<VoronoiCell> GetVoronoiCellsBasedOnCentroids() => GetVoronoiCells(GetCentroid);
        public IEnumerable<Edge> GetHullEdges() => CreateHull(GetHullPoints());
        public Point[] GetHullPoints() => Array.ConvertAll(Hull, (x) => Points[x]);

        public Point[] GetTrianglePoints(int t)
        {
            List<Point> points = new List<Point>();

            foreach (int p in PointsOfTriangle(t))
            {
                points.Add(Points[p]);
            }

            return points.ToArray();
        }

        public Point[] GetRellaxedPoints()
        {
            List<Point> points = new List<Point>();

            foreach (VoronoiCell cell in GetVoronoiCellsBasedOnCircumcenters())
            {
                points.Add(GetCentroid(cell.Points));
            }

            return points.ToArray();
        }

        public IEnumerable<Edge> GetEdgesOfTriangle(int t)
        {
            return CreateHull(EdgesOfTriangle(t).Select(e => Points[Triangles[e]]));
        }

        public Point GetTriangleCircumcenter(int t)
        {
            Point[] vertices = GetTrianglePoints(t);
            return GetCircumcenter(vertices[0], vertices[1], vertices[2]);
        }

        public Point GetCentroid(int t)
        {
            Point[] vertices = GetTrianglePoints(t);
            return GetCentroid(vertices);
        }

        public void ForEachTriangle(Action<Triangle> callback)
        {
            foreach (Triangle triangle in GetTriangles())
            {
                callback?.Invoke(triangle);
            }
        }

        public void ForEachTriangleEdge(Action<Edge> callback)
        {
            foreach (Edge edge in GetEdges())
            {
                callback?.Invoke(edge);
            }
        }

        public void ForEachVoronoiEdge(Action<Edge> callback)
        {
            foreach (Edge edge in GetVoronoiEdges())
            {
                callback?.Invoke(edge);
            }
        }

        public void ForEachVoronoiCellBasedOnCentroids(Action<VoronoiCell> callback)
        {
            foreach (VoronoiCell cell in GetVoronoiCellsBasedOnCentroids())
            {
                callback?.Invoke(cell);
            }
        }

        public void ForEachVoronoiCellBasedOnCircumcenters(Action<VoronoiCell> callback)
        {
            foreach (VoronoiCell cell in GetVoronoiCellsBasedOnCircumcenters())
            {
                callback?.Invoke(cell);
            }
        }

        public void ForEachVoronoiCell(Action<VoronoiCell> callback, Func<int, Point> triangleVertexSelector = null)
        {
            foreach (VoronoiCell cell in GetVoronoiCells(triangleVertexSelector))
            {
                callback?.Invoke(cell);
            }
        }

        public IEnumerable<int> EdgesAroundPoint(int start)
        {
            int incoming = start;
            do
            {
                yield return incoming;
                int outgoing = NextHalfedge(incoming);
                incoming = HalfEdges[outgoing];
            } while (incoming != -1 && incoming != start);
        }

        public IEnumerable<int> PointsOfTriangle(int t)
        {
            foreach (int edge in EdgesOfTriangle(t))
            {
                yield return Triangles[edge];
            }
        }

        public IEnumerable<int> TrianglesAdjacentToTriangle(int t)
        {
            List<int> adjacentTriangles = new List<int>();
            int[] triangleEdges = EdgesOfTriangle(t);

            foreach (int e in triangleEdges)
            {
                int opposite = HalfEdges[e];
                if (opposite >= 0)
                {
                    adjacentTriangles.Add(TriangleOfEdge(opposite));
                }
            }

            return adjacentTriangles;
        }

        public static int NextHalfedge(int e) => (e % 3 == 2) ? e - 2 : e + 1;
        public static int PreviousHalfedge(int e) => (e % 3 == 0) ? e + 2 : e - 1;
        public static int[] EdgesOfTriangle(int t) => new int[] { 3 * t, 3 * t + 1, 3 * t + 2 };
        public static int TriangleOfEdge(int e) => e / 3;
        public static IEnumerable<Edge> CreateHull(IEnumerable<Point> points) => points.Zip(points.Skip(1).Append(points.FirstOrDefault()), (a, b) => new Edge(0, a, b)).OfType<Edge>();
        public static Point GetCircumcenter(Point a, Point b, Point c) => Circumcenter(a.X, a.Y, b.X, b.Y, c.X, c.Y);

        public static Point GetCentroid(Point[] points)
        {
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                float temp = points[i].X * points[j].Y - points[j].X * points[i].Y;
                accumulatedArea += temp;
                centerX += (points[i].X + points[j].X) * temp;
                centerY += (points[i].Y + points[j].Y) * temp;
            }

            if (Math.Abs(accumulatedArea) < 1E-7f)
            {
                return new Point();
            }

            accumulatedArea *= 3f;
            return new Point(centerX / accumulatedArea, centerY / accumulatedArea);
        }
    }
}
