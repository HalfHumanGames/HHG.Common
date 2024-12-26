using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace HHG.Common.Runtime
{
    public class AStar
    {
        private static readonly Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(-1, -1, 0)
        };

        public static void FindPath(Vector3Int start, Vector3Int goal, HashSet<Vector3Int> obstacles, List<Vector3Int> path)
        {
            if (obstacles == null)
            {
                throw new System.ArgumentNullException(nameof(obstacles));
            }

            if (path == null)
            {
                throw new System.ArgumentNullException(nameof(path));
            }

            path.Clear();

            var openSet = PriorityQueuePool<Vector3Int, float>.Get();
            var cameFrom = DictionaryPool<Vector3Int, Vector3Int>.Get();
            var gScore = DictionaryPool<Vector3Int, float>.Get();
            var fScore = DictionaryPool<Vector3Int, float>.Get();

            gScore.Add(start, 0);
            fScore.Add(start, Heuristic(start, goal));

            openSet.Enqueue(start, fScore[start]);

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                if (current == goal)
                {
                    ReconstructPath(cameFrom, current, path);
                    break;
                }

                foreach (var direction in directions)
                {
                    var neighbor = current + direction;
                    if (obstacles.Contains(neighbor))
                    {
                        continue;
                    }

                    float tentativeGScore = gScore[current] + ((direction.x != 0 && direction.y != 0) ? 1.5f : 1f);

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                        }
                    }
                }
            }

            PriorityQueuePool<Vector3Int, float>.Release(openSet);
            DictionaryPool<Vector3Int, Vector3Int>.Release(cameFrom);
            DictionaryPool<Vector3Int, float>.Release(gScore);
            DictionaryPool<Vector3Int, float>.Release(fScore);
        }

        private static float Heuristic(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private static List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current, List<Vector3Int> path)
        {
            path.Clear();
            path.Add(current);
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    } 
}