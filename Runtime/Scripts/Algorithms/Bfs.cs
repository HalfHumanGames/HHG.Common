using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace HHG.Common.Runtime
{
    public class Bfs
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

        public static void Fill(Vector3Int start, float range, HashSet<Vector3Int> obstacles, HashSet<Vector3Int> fill)
        {
            if (obstacles == null)
            {
                throw new System.ArgumentNullException(nameof(obstacles));
            }

            if (fill == null)
            {
                throw new System.ArgumentNullException(nameof(fill));
            }

            fill.Clear();

            var queue = QueuePool<(Vector3Int position, float cost)>.Get();
            var visited = HashSetPool<Vector3Int>.Get();

            queue.Enqueue((start, 0));
            visited.Add(start);

            while (queue.Count > 0)
            {
                var (current, currentCost) = queue.Dequeue();

                foreach (var direction in directions)
                {
                    var neighbor = current + direction;
                    if (visited.Contains(neighbor) || obstacles.Contains(neighbor))
                    {
                        continue;
                    }

                    float additionalCost = (direction.x != 0 && direction.y != 0) ? 1.5f : 1f;
                    float newCost = currentCost + additionalCost;

                    if (newCost <= range)
                    {
                        queue.Enqueue((neighbor, newCost));
                        visited.Add(neighbor);
                        fill.Add(neighbor);
                    }
                }
            }

            QueuePool<(Vector3Int position, float cost)>.Release(queue);
            HashSetPool<Vector3Int>.Release(visited);
        }
    }
}