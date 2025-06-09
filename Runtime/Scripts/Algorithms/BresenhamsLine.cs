using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class BresenhamsLine
    {
        public static bool CanDrawLine(Vector3Int start, Vector3Int end, HashSet<Vector3Int> obstacles)
        {
            int x0 = start.x;
            int y0 = start.y;
            int x1 = end.x;
            int y1 = end.y;

            int dx = Mathf.Abs(x1 - x0);
            int dy = -Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;

            while (true)
            {
                if (x0 == x1 && y0 == y1) break;

                // Check obstacled after check current location
                if (obstacles.Contains(new Vector3Int(x0, y0))) return false;

                int e2 = 2 * err;
                if (e2 >= dy)
                {
                    err += dy;
                    x0 += sx;
                }
                if (e2 <= dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return true;
        }
    }
}