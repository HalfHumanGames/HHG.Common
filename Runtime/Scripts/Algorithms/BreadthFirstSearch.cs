using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace HHG.Common.Runtime
{
    public class BreadthFirstSearch
    {
        private static readonly Vector3Int[] cardinalDirections = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
        };

        private static readonly Vector3Int[] diagonalDirections = new Vector3Int[]
        {
            new Vector3Int(1, 1, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(-1, -1, 0)
        };

        private struct Node
        {
            public Vector3Int Position;
            public float Cost;

            public Node(Vector3Int position, float cost)
            {
                Position = position;
                Cost = cost;
            }
        }

        public static void Fill(
            Vector3Int start,
            float range,
            ICollection<Vector3Int> obstacles,
            ICollection<Vector3Int> fill,
            bool useDiagonal = true,
            float cardinalCost = 1f,
            float diagonalCost = 1.5f,
            Dictionary<Vector3Int, float> costMap = null)
        {
            if (obstacles == null) throw new System.ArgumentNullException(nameof(obstacles));
            if (fill == null) throw new System.ArgumentNullException(nameof(fill));

            fill.Clear();
            costMap?.Clear();

            var queue = QueuePool<Node>.Get();
            var visited = HashSetPool<Vector3Int>.Get();

            queue.Enqueue(new Node(start, 0));
            visited.Add(start);
            costMap?.Add(start, 0f);

            var directions = Enumerable.Empty<Vector3Int>().Concat(cardinalDirections);
            if (useDiagonal) directions.Concat(diagonalDirections);

            while (queue.Count > 0)
            {
                Node node = queue.Dequeue();

                foreach (var direction in directions)
                {
                    var neighbor = node.Position + direction;

                    if (visited.Contains(neighbor) || obstacles.Contains(neighbor))
                    {
                        continue;
                    }

                    bool isDiagonal = direction.x != 0 && direction.y != 0;
                    float stepCost = isDiagonal ? diagonalCost : cardinalCost;
                    float newCost = node.Cost + stepCost;

                    if (newCost <= range)
                    {
                        queue.Enqueue(new Node(neighbor, newCost));
                        visited.Add(neighbor);
                        fill.Add(neighbor);
                        costMap?.Add(neighbor, newCost);
                    }
                }
            }

            QueuePool<Node>.Release(queue);
            HashSetPool<Vector3Int>.Release(visited);
        }
    }
}
