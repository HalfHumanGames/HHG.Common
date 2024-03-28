using HHG.Common.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    public class FloodFill
    {
        private Queue<Vector3Int> queue;
        private int offsetX;
        private int offsetY;
        private int maxX;
        private int maxY;
        private bool[,] visited;
        private bool[,] obstacles;
        private bool diagonal;

        public FloodFill(Tilemap tilemap, BoundsInt bounds, FloodFillMode mode, bool fillDiagonal, params TileBase[] tileBases) : this(tilemap, bounds, mode, fillDiagonal, (t, p) => t.HasTile(p, tileBases))
        {
            
        }

        public FloodFill(Tilemap tilemap, BoundsInt bounds, FloodFillMode mode, bool fillDiagonal, params TileTagAsset[] tileTags) : this(tilemap, bounds, mode, fillDiagonal, (t, p) => t.HasTile(p, tileTags))
        {

        }

        public FloodFill(Tilemap tilemap, BoundsInt bounds, FloodFillMode mode, bool fillDiagonal, Func<Tilemap, Vector3Int, bool> evaluator)
        {
            int width = bounds.size.x;
            int height = bounds.size.y;

            queue = new Queue<Vector3Int>();
            offsetX = -bounds.min.x;
            offsetY = -bounds.min.y;
            maxX = width - 1;
            maxY = height - 1;
            visited = new bool[width, height];
            obstacles = new bool[width, height];
            diagonal = fillDiagonal;

            foreach (Vector3Int position in bounds.allPositionsWithin)
            {
                int x = position.x + offsetX;
                int y = position.y + offsetY;
                bool hasTile = evaluator(tilemap, position);
                obstacles[x, y] = mode == FloodFillMode.Obstacle ? hasTile : !hasTile;
            }
        }

        public void Fill(Vector3Int startPosition, FloodFillResult result)
        {
            result.Reset();
            queue.Clear();
            queue.Enqueue(startPosition);

            while (queue.Count > 0)
            {
                Vector3Int position = queue.Dequeue();

                int x = position.x + offsetX;
                int y = position.y + offsetY;

                if (IsOutOfBoundsByIndices(x, y))
                {
                    result.AreaBordersEdge = true;
                    continue;
                }

                if (CanSkipPositionByIndices(x, y))
                {
                    continue;
                }

                visited[x, y] = true;
                result.Area.Add(position);

                // Enqueue neighboring positions
                queue.Enqueue(new Vector3Int(position.x, position.y + 1));
                queue.Enqueue(new Vector3Int(position.x, position.y - 1));
                queue.Enqueue(new Vector3Int(position.x + 1, position.y));
                queue.Enqueue(new Vector3Int(position.x - 1, position.y));
                
                if (diagonal)
                {
                    queue.Enqueue(new Vector3Int(position.x + 1, position.y + 1));
                    queue.Enqueue(new Vector3Int(position.x + 1, position.y - 1));
                    queue.Enqueue(new Vector3Int(position.x - 1, position.y + 1));
                    queue.Enqueue(new Vector3Int(position.x - 1, position.y - 1));
                }
            }
        }

        public bool IsOutOfBoundsByIndices(int x, int y)
        {
            return x < 0 || y < 0 || x > maxX || y > maxY;
        }

        public bool CanSkipPosition(int x, int y)
        {
            return CanSkipPositionByIndices(x + offsetX, y + offsetY);
        }

        public bool CanSkipPositionByIndices(int x, int y)
        {
            return visited[x, y] || obstacles[x, y];
        }
    }
}