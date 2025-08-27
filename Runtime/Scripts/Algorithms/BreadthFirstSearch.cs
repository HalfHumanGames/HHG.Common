using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class BreadthFirstSearch
    {
        private static readonly Vector3Int[] directions4 = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
        };

        private static readonly Vector3Int[] directions8 = new Vector3Int[]
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

        private static readonly Queue<Node> queue = new();
        private static readonly HashSet<Vector3Int> visited = new();

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
            ISet<Vector3Int> obstacles, // Use ISet for O(1) lookups
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

            queue.Clear();
            visited.Clear();

            queue.Enqueue(new Node(start, 0));
            visited.Add(start);
            costMap?.Add(start, 0f);

            Vector3Int[] directions = useDiagonal ? directions8 : directions4;

            while (queue.Count > 0)
            {
                Node node = queue.Dequeue();

                for (int i = 0; i < directions.Length; i++)
                {
                    Vector3Int neighbor = node.Position;
                    neighbor.x += directions[i].x;
                    neighbor.y += directions[i].y;

                    // fast check if already visited
                    if (!visited.Add(neighbor)) continue;

                    if (obstacles.Contains(neighbor)) continue;

                    bool isDiagonal = i >= 4;
                    float stepCost = isDiagonal ? diagonalCost : cardinalCost;
                    float newCost = node.Cost + stepCost;

                    if (newCost <= range)
                    {
                        queue.Enqueue(new Node(neighbor, newCost));
                        fill.Add(neighbor);
                        costMap?.Add(neighbor, newCost);
                    }
                }
            }
        }
    }
}
