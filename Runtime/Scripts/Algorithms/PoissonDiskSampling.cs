using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class PoissonDiskSampling
    {
        private const float invertRoot2 = 0.70710678118f;
        private const int defaultIterationPerPoint = 30;

        private class Settings
        {
            public Vector2 BottomLeft;
            public Vector2 TopRight;
            public Vector2 Center;
            public Rect Dimension;

            public float MinimumDistance;
            public int IterationPerPoint;

            public float CellSize;
            public int GridWidth;
            public int GridHeight;
        }

        private class Bags
        {
            public Vector2?[,] Grid;
            public List<Vector2> SamplePoints;
            public List<Vector2> ActivePoints;
        }

        public static List<Vector2> Sampling(Vector2 bottomLeft, Vector2 topRight, float minimumDistance)
        {
            return Sampling(bottomLeft, topRight, minimumDistance, defaultIterationPerPoint);
        }

        public static List<Vector2> Sampling(Vector2 bottomLeft, Vector2 topRight, float minimumDistance, int iterationPerPoint)
        {
            Settings settings = GetSettings(bottomLeft, topRight, minimumDistance, iterationPerPoint <= 0 ? defaultIterationPerPoint : iterationPerPoint);

            System.Random random = new System.Random();

            Bags bags = new Bags()
            {
                Grid = new Vector2?[settings.GridWidth + 1, settings.GridHeight + 1],
                SamplePoints = new List<Vector2>(),
                ActivePoints = new List<Vector2>()
            };

            GetFirstPoint(settings, bags, random);

            do
            {
                int index = random.Next(0, bags.ActivePoints.Count);
                Vector2 point = bags.ActivePoints[index];
                bool found = false;

                for (int i = 0; i < settings.IterationPerPoint; i++)
                {
                    found = found | GetNextPoint(point, settings, bags, random);
                }

                if (!found)
                {
                    bags.ActivePoints.RemoveAt(index);
                }
            }
            while (bags.ActivePoints.Count > 0);

            return bags.SamplePoints;
        }

        private static bool GetNextPoint(Vector2 point, Settings set, Bags bags, System.Random random)
        {
            bool found = false;

            point += GetRandPosInCircle(set.MinimumDistance, 2f * set.MinimumDistance, random);

            if (!set.Dimension.Contains(point))
            {
                return false;
            }

            float minimum = set.MinimumDistance * set.MinimumDistance;
            Vector2Int index = GetGridIndex(point, set);
            bool drop = false;

            int around = 2;
            Vector2Int fieldMin = new Vector2Int(Mathf.Max(0, index.x - around), Mathf.Max(0, index.y - around));
            Vector2Int fieldMax = new Vector2Int(Mathf.Min(set.GridWidth, index.x + around), Mathf.Min(set.GridHeight, index.y + around));

            for (int i = fieldMin.x; i <= fieldMax.x && !drop; i++)
            {
                for (int j = fieldMin.y; j <= fieldMax.y && !drop; j++)
                {
                    Vector2? q = bags.Grid[i, j];
                    if (q.HasValue && (q.Value - point).sqrMagnitude <= minimum)
                    {
                        drop = true;
                    }
                }
            }

            if (!drop)
            {
                found = true;
                bags.SamplePoints.Add(point);
                bags.ActivePoints.Add(point);
                bags.Grid[index.x, index.y] = point;
            }

            return found;
        }

        private static void GetFirstPoint(Settings set, Bags bags, System.Random random)
        {
            Vector2 first = new Vector2(
                (float)(random.NextDouble() * (set.TopRight.x - set.BottomLeft.x) + set.BottomLeft.x),
                (float)(random.NextDouble() * (set.TopRight.y - set.BottomLeft.y) + set.BottomLeft.y)
            );

            Vector2Int index = GetGridIndex(first, set);
            bags.Grid[index.x, index.y] = first;
            bags.SamplePoints.Add(first);
            bags.ActivePoints.Add(first);
        }

        private static Vector2Int GetGridIndex(Vector2 point, Settings set)
        {
            return new Vector2Int(
                Mathf.FloorToInt((point.x - set.BottomLeft.x) / set.CellSize),
                Mathf.FloorToInt((point.y - set.BottomLeft.y) / set.CellSize)
            );
        }

        private static Settings GetSettings(Vector2 bl, Vector2 tr, float min, int iteration)
        {
            Vector2 dimension = (tr - bl);
            float cell = min * invertRoot2;

            return new Settings()
            {
                BottomLeft = bl,
                TopRight = tr,
                Center = (bl + tr) * 0.5f,
                Dimension = new Rect(new Vector2(bl.x, bl.y), new Vector2(dimension.x, dimension.y)),

                MinimumDistance = min,
                IterationPerPoint = iteration,

                CellSize = cell,
                GridWidth = Mathf.CeilToInt(dimension.x / cell),
                GridHeight = Mathf.CeilToInt(dimension.y / cell)
            };
        }

        private static Vector2 GetRandPosInCircle(float fieldMin, float fieldMax, System.Random random)
        {
            float theta = (float)random.NextDouble() * Mathf.PI * 2;
            float radius = Mathf.Sqrt((float)random.NextDouble() * (fieldMax * fieldMax - fieldMin * fieldMin) + fieldMin * fieldMin);

            return new Vector2((float)(radius * Mathf.Cos(theta)), (float)(radius * Mathf.Sin(theta)));
        }
    }
}
