using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    public static class VectorExtensions
    {
        public static Vector2 Round(this Vector2 v, float nearest = 1f)
        {
            return new Vector2(
                v.x.RoundToNearest(nearest),
                v.y.RoundToNearest(nearest)
            );
        }

        public static Vector3 Round(this Vector3 v, float nearest = 1f)
        {
            return new Vector3(
                v.x.RoundToNearest(nearest),
                v.y.RoundToNearest(nearest),
                v.z.RoundToNearest(nearest)
            );
        }

        public static Vector2Int RoundToInt(this Vector2 v, float nearest = 1f)
        {
            return new Vector2Int(
                (int)v.x.RoundToNearest(nearest),
                (int)v.y.RoundToNearest(nearest)
            );
        }

        public static Vector3Int RoundToInt(this Vector3 v, float nearest = 1f)
        {
            return new Vector3Int(
                (int)v.x.RoundToNearest(nearest),
                (int)v.y.RoundToNearest(nearest),
                (int)v.z.RoundToNearest(nearest)
            );
        }

        public static Vector2 WithX(this Vector2 vec3, float x)
        {
            vec3.x = x;
            return vec3;
        }

        public static Vector2 WithY(this Vector2 vec3, float y)
        {
            vec3.y = y;
            return vec3;
        }

        public static Vector3 WithX(this Vector3 vec3, float x)
        {
            vec3.x = x;
            return vec3;
        }

        public static Vector3 WithY(this Vector3 vec3, float y)
        {
            vec3.y = y;
            return vec3;
        }

        public static Vector3 WithZ(this Vector3 vec3, float z)
        {
            vec3.z = z;
            return vec3;
        }

        public static Vector2Int WithX(this Vector2Int vec3, int x)
        {
            vec3.x = x;
            return vec3;
        }

        public static Vector2Int WithY(this Vector2Int vec3, int y)
        {
            vec3.y = y;
            return vec3;
        }

        public static Vector3Int WithX(this Vector3Int vec3, int x)
        {
            vec3.x = x;
            return vec3;
        }

        public static Vector3Int WithY(this Vector3Int vec3, int y)
        {
            vec3.y = y;
            return vec3;
        }

        public static Vector3Int WithZ(this Vector3Int vec3, int z)
        {
            vec3.z = z;
            return vec3;
        }

        public static Vector2Int[] GetAdjacentPositions(this Vector2Int position, bool includeDiagonals = false)
        {
            int i = 0;
            Vector2Int[] retval = new Vector2Int[includeDiagonals ? 8 : 4];
            BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
            foreach (Vector2Int offset in bounds.allPositionsWithin)
            {
                if (offset.x == 0 && offset.y == 0) continue;

                if (!includeDiagonals && offset.x != 0 && offset.y != 0) continue;

                retval[i++] = position + offset;
            }
            return retval;
        }

        public static Vector3Int[] GetAdjacentPositions(this Vector3Int position, bool includeDiagonals = false)
        {
            int i = 0;
            Vector3Int[] retval = new Vector3Int[includeDiagonals ? 8 : 4];
            BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
            foreach (Vector3Int offset in bounds.allPositionsWithin)
            {
                if (offset.x == 0 && offset.y == 0) continue;

                if (!includeDiagonals && offset.x != 0 && offset.y != 0) continue;

                retval[i++] = position + offset;
            }
            return retval;
        }

        public static Vector3Int[] GetAdjacentPositions(this BoundsInt bounds, bool includeDiagonals = false)
        {
            int i = 0;
            int len = (bounds.size.x * 2) + (bounds.size.y * 2);
            Vector3Int[] retval = new Vector3Int[includeDiagonals ? len + 4 : len];
            bounds.min -= new Vector3Int(1, 1);
            bounds.max += new Vector3Int(1, 1);
            foreach (Vector3Int position in bounds.allPositionsWithin)
            {
                bool xInside = position.x != bounds.min.x && position.x != bounds.max.x;
                bool yInside = position.y != bounds.min.y && position.y != bounds.max.y;

                if (xInside && yInside) continue;

                if (!includeDiagonals && !xInside && !yInside) continue;

                retval[i++] = position;
            }
            return retval;
        }
    }
}