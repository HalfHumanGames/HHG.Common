using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class AStar
    {
        private static readonly List<(Vector3Int, float)> directionBuffer = new List<(Vector3Int, float)>(8);
        private static readonly PriorityQueue<Vector3Int, float> openSet = new PriorityQueue<Vector3Int, float>();
        private static readonly Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        private static readonly Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float>();
        private static readonly Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float>();

        public static bool FindPath(
            Vector3Int start,
            Vector3Int goal,
            ISet<Vector3Int> obstacles, // use ISet for O(1) Contains
            List<Vector3Int> path,
            float cardinalCost,
            float diagonalCost)
        {
            if (obstacles == null) throw new System.ArgumentNullException(nameof(obstacles));
            if (path == null) throw new System.ArgumentNullException(nameof(path));

            path.Clear();

            openSet.Clear();
            cameFrom.Clear();
            gScore.Clear();
            fScore.Clear();

            gScore[start] = 0f;
            fScore[start] = Heuristic(start, goal, cardinalCost, diagonalCost);
            openSet.Enqueue(start, fScore[start]);

            BuildDirections(cardinalCost, diagonalCost, directionBuffer);

            while (openSet.Count > 0)
            {
                Vector3Int current = openSet.Dequeue();

                // Skip stale nodes
                float currentG = gScore[current];
                float expectedF = currentG + Heuristic(current, goal, cardinalCost, diagonalCost);
                if (fScore[current] < expectedF) continue;

                if (current == goal)
                {
                    ReconstructPath(cameFrom, current, path);
                    return true;
                }

                foreach ((Vector3Int dir, float stepCost) in directionBuffer)
                {
                    Vector3Int neighbor = current + dir;

                    if (obstacles.Contains(neighbor)) continue;

                    float tentativeG = currentG + stepCost;

                    if (!gScore.TryGetValue(neighbor, out float oldG) || tentativeG < oldG)
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        float h = Heuristic(neighbor, goal, cardinalCost, diagonalCost);
                        fScore[neighbor] = tentativeG + h;
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }

            return false;
        }

        private static float Heuristic(Vector3Int a, Vector3Int b, float cardinalCost, float ordinalCost)
        {
            float dx = Mathf.Abs(a.x - b.x);
            float dy = Mathf.Abs(a.y - b.y);
            return cardinalCost * (dx + dy) + (ordinalCost - 2f * cardinalCost) * Mathf.Min(dx, dy);
        }

        private static List<Vector3Int> ReconstructPath(
            Dictionary<Vector3Int, Vector3Int> cameFrom,
            Vector3Int current,
            List<Vector3Int> path)
        {
            path.Clear();
            path.Add(current);

            while (cameFrom.TryGetValue(current, out Vector3Int parent))
            {
                current = parent;
                path.Add(current);
            }

            path.Reverse();
            return path;
        }

        private static void BuildDirections(float cardinalCost, float ordinalCost, List<(Vector3Int, float)> buffer)
        {
            buffer.Clear();
            buffer.Add((new Vector3Int(1, 0, 0), cardinalCost));
            buffer.Add((new Vector3Int(-1, 0, 0), cardinalCost));
            buffer.Add((new Vector3Int(0, 1, 0), cardinalCost));
            buffer.Add((new Vector3Int(0, -1, 0), cardinalCost));
            buffer.Add((new Vector3Int(1, 1, 0), ordinalCost));
            buffer.Add((new Vector3Int(-1, 1, 0), ordinalCost));
            buffer.Add((new Vector3Int(1, -1, 0), ordinalCost));
            buffer.Add((new Vector3Int(-1, -1, 0), ordinalCost));
        }
    }
}
