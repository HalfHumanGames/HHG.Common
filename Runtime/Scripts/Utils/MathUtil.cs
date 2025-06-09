using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class MathUtil
    {
        public static float ManhattanDistance(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public static float ChebyshevDistance(Vector2 a, Vector2 b)
        {
            return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
        }

        public static float SquaredDifference(Vector2 a, Vector2 b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            return x * x + y * y;
        }

        public static float ManhattanDistance(Vector3 a, Vector3 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public static float ChebyshevDistance(Vector3 a, Vector3 b)
        {
            return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
        }

        public static float SquaredDifference(Vector3 a, Vector3 b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            return x * x + y * y;
        }

        public static float ManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public static float ChebyshevDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
        }

        public static int SquaredDifference(Vector2Int a, Vector2Int b)
        {
            int x = a.x - b.x;
            int y = a.y - b.y;
            return x * x + y * y;
        }

        public static float GridDistance(Vector2Int a, Vector2Int b, float cardinalCost = 1f, float diagonalCost = 1.5f)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            int diagonalSteps = Mathf.Min(dx, dy);
            int straightSteps = Mathf.Abs(dx - dy);
            return diagonalSteps * diagonalCost + straightSteps * cardinalCost;
        }

        public static float ManhattanDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public static float ChebyshevDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
        }

        public static int SquaredDifference(Vector3Int a, Vector3Int b)
        {
            int x = a.x - b.x;
            int y = a.y - b.y;
            return x * x + y * y;
        }

        public static float GridDistance(Vector3Int a, Vector3Int b, float cardinalCost = 1f, float diagonalCost = 1.5f)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            int diagonalSteps = Mathf.Min(dx, dy);
            int straightSteps = Mathf.Abs(dx - dy);
            return diagonalSteps * diagonalCost + straightSteps * cardinalCost;
        }

        public static int RoundToMultiple(int value, int multiple)
        {
            if (multiple == 0)
            {
                return value;
            }

            return Mathf.RoundToInt((float)value / multiple) * multiple;
        }

        public static float RoundToMultiple(float value, int multiple)
        {
            if (multiple == 0)
            {
                return value;
            }

            return Mathf.Round(value / multiple) * multiple;
        }
    }
}