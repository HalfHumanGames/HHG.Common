using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class VectorExtensions
    {
        public static Vector2[] directionsCompassV2 = new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(-1, 1),
            new Vector2(-1, 0),
            new Vector2(-1, -1),
            new Vector2(0, -1),
            new Vector2(1, -1)
        };

        public static Vector2[] directionsCardinalV2 = new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(-1, 0),
            new Vector2(0, -1)
        };

        public static Vector2[] directionsOrdinalV2 = new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(1, -1),
            new Vector2(-1, 1),
            new Vector2(-1, -1)
        };

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

        public static bool IsStraightLine(this IEnumerable<Vector2Int> points)
        {
            using (var enumerator = points.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return true; // No points, considered trivially collinear
                }

                Vector2Int firstPoint = enumerator.Current;

                if (!enumerator.MoveNext())
                {
                    return true; // Only one point, considered trivially collinear
                }

                Vector2Int secondPoint = enumerator.Current;
                Vector2Int start = secondPoint - firstPoint;

                Vector2Int previousPoint = secondPoint;
                while (enumerator.MoveNext())
                {
                    Vector2Int currentPoint = enumerator.Current;
                    Vector2Int next = currentPoint - previousPoint;
                    int cross = start.x * next.y - start.y * next.x;
                    if (cross != 0)
                    {
                        return false;
                    }
                    previousPoint = currentPoint;
                }
            }
            return true;
        }

        public static bool IsStraightLine(this IEnumerable<Vector3Int> points)
        {
            using (var enumerator = points.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return true; // No points, considered trivially collinear
                }

                Vector3Int firstPoint = enumerator.Current;

                if (!enumerator.MoveNext())
                {
                    return true; // Only one point, considered trivially collinear
                }

                Vector3Int secondPoint = enumerator.Current;
                Vector3 start = (Vector3)(secondPoint - firstPoint);

                Vector3Int previousPoint = secondPoint;
                while (enumerator.MoveNext())
                {
                    Vector3Int currentPoint = enumerator.Current;
                    Vector3 next = (Vector3)(currentPoint - previousPoint);
                    Vector3 cross = Vector3.Cross(start, next);
                    if (cross != Vector3.zero)
                    {
                        return false;
                    }
                    previousPoint = currentPoint;
                }
            }
            return true;
        }

        public static bool IsAdjacent(this Vector2Int a, Vector2Int b, bool includeDiagonals = false)
        {
            int deltaX = Mathf.Abs(a.x - b.x);
            int deltaY = Mathf.Abs(a.y - b.y);

            if (includeDiagonals)
            {
                return deltaX <= 1 && deltaY <= 1 && (deltaX + deltaY) > 0;
            }
            else
            {
                return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
            }
        }

        public static bool IsAdjacent(this Vector3Int a, Vector3Int b, bool includeDiagonals = false)
        {
            int deltaX = Mathf.Abs(a.x - b.x);
            int deltaY = Mathf.Abs(a.y - b.y);
            int deltaZ = Mathf.Abs(a.z - b.z);

            if (includeDiagonals)
            {
                return deltaX <= 1 && deltaY <= 1 && deltaZ <= 1 && (deltaX + deltaY + deltaZ) > 0;
            }
            else
            {
                return (deltaX == 1 && deltaY == 0 && deltaZ == 0) ||
                       (deltaX == 0 && deltaY == 1 && deltaZ == 0) ||
                       (deltaX == 0 && deltaY == 0 && deltaZ == 1);
            }
        }

        public static Vector2Int SnapToNearestDirectionCompass(this Vector3 direction)
        {
            return direction.SnapToNearestDirection(directionsCompassV2);
        }

        public static Vector2Int SnapToNearestDirectionCardinal(this Vector3 direction)
        {
            return direction.SnapToNearestDirection(directionsCardinalV2);
        }

        public static Vector2Int SnapToNearestDirectionOrdinal(this Vector3 direction)
        {
            return direction.SnapToNearestDirection(directionsOrdinalV2);
        }

        public static Vector2Int SnapToNearestDirectionCompass(this Vector2 direction)
        {
            return direction.SnapToNearestDirection(directionsCompassV2);
        }

        public static Vector2Int SnapToNearestDirectionCardinal(this Vector2 direction)
        {
            return direction.SnapToNearestDirection(directionsCardinalV2);
        }

        public static Vector2Int SnapToNearestDirectionOrdinal(this Vector2 direction)
        {
            return direction.SnapToNearestDirection(directionsOrdinalV2);
        }

        public static Vector2Int SnapToNearestDirection(this Vector3 direction, Vector2[] directions)
        {
            return ((Vector2)direction).SnapToNearestDirection(directions);
        }

        public static Vector2Int SnapToNearestDirection(this Vector2 direction, Vector2[] directions)
        {
            direction.Normalize();
            Vector2 closestDirection = Vector2.zero;
            float closestDistance = float.MaxValue;

            foreach (Vector2 dir in directions)
            {
                float distance = Vector2.Distance(direction, dir);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestDirection = dir;
                }
            }

            return new Vector2Int(Mathf.RoundToInt(closestDirection.x), Mathf.RoundToInt(closestDirection.y));
        }

        public static float ToAngle(this Vector3 direction)
        {
            // Calculate the angle in radians and convert to degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Ensure the angle is in the range [0, 360)
            if (angle < 0) angle += 360;

            return angle;
        }

        public static float ToAngle(this Vector2 direction)
        {
            // Calculate the angle in radians and convert to degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Ensure the angle is in the range [0, 360)
            if (angle < 0) angle += 360;

            return angle;
        }


        public static float ToAngle(this Vector3Int direction)
        {
            // Calculate the angle in radians and convert to degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Ensure the angle is in the range [0, 360)
            if (angle < 0) angle += 360;

            return angle;
        }

        public static float ToAngle(this Vector2Int direction)
        {
            // Calculate the angle in radians and convert to degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Ensure the angle is in the range [0, 360)
            if (angle < 0) angle += 360;

            return angle;
        }

        public static Vector2 Average(this IEnumerable<Vector2Int> vectors)
        {
            int count = vectors.Count();

            if (count == 0) return Vector2Int.zero;

            Vector2Int sum = vectors.Aggregate(Vector2Int.zero, (acc, v) => acc + v);

            return new Vector2(sum.x / count, sum.y / count);

        }

        public static Vector3 Average(this IEnumerable<Vector3Int> vectors)
        {
            int count = vectors.Count();

            if (count == 0) return Vector3Int.zero;

            Vector3Int sum = vectors.Aggregate(Vector3Int.zero, (acc, v) => acc + v);

            return new Vector3(sum.x / count, sum.y / count, sum.z / count);

        }

        public static Vector2Int AverageToInt(this IEnumerable<Vector2Int> vectors)
        {
            int count = vectors.Count();

            if (count == 0) return Vector2Int.zero;

            Vector2Int sum = vectors.Aggregate(Vector2Int.zero, (acc, v) => acc + v);

            return new Vector2Int(
                Mathf.RoundToInt((float)sum.x / count),
                Mathf.RoundToInt((float)sum.y / count)
            );
        }

        public static Vector3Int AverageToInt(this IEnumerable<Vector3Int> vectors)
        {
            int count = vectors.Count();

            if (count == 0) return Vector3Int.zero;

            Vector3Int sum = vectors.Aggregate(Vector3Int.zero, (acc, v) => acc + v);

            return new Vector3Int(
                Mathf.RoundToInt((float)sum.x / count),
                Mathf.RoundToInt((float)sum.y / count),
                Mathf.RoundToInt((float)sum.z / count)
            );
        }

        public static Vector2 Average(this IEnumerable<Vector2> vectors)
        {
            int count = vectors.Count();

            if (count == 0) return Vector2.zero;

            Vector2 sum = vectors.Aggregate(Vector2.zero, (acc, v) => acc + v);

            return new Vector2(sum.x / count, sum.y / count);
        }

        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            int count = vectors.Count();

            if (count == 0) return Vector3.zero;

            Vector3 sum = vectors.Aggregate(Vector3.zero, (acc, v) => acc + v);

            return new Vector3(sum.x / count, sum.y / count, sum.y / count);
        }
    }
}