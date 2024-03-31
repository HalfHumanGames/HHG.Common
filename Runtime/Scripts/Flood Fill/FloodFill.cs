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
        private bool filldiagonal;
        private bool fillobstacle;

        public FloodFill(Tilemap tilemap, BoundsInt bounds, FloodFillMode mode, bool fillDiagonal, bool fillObstacle, params TileBase[] tileBases) : this(tilemap, bounds, mode, fillDiagonal, fillObstacle, (t, p) => t.HasTile(p, tileBases))
        {
            
        }

        public FloodFill(Tilemap tilemap, BoundsInt bounds, FloodFillMode mode, bool fillDiagonal, bool fillObstacle, params TileTagAsset[] tileTags) : this(tilemap, bounds, mode, fillDiagonal, fillObstacle, (t, p) => t.HasTile(p, tileTags))
        {

        }

        public FloodFill(Tilemap tilemap, BoundsInt bounds, FloodFillMode mode, bool fillDiagonal, bool fillObstacle, Func<Tilemap, Vector3Int, bool> evaluator)
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
            filldiagonal = fillDiagonal;
            fillobstacle = fillObstacle;

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

                // Make sure to perform out of bounds check first
                if (IsIndexOutOfBounds(x, y))
                {
                    result.AreaBordersEdge = true;
                    continue;
                }

                if (HasVisitedIndex(x, y))
                {
                    continue;
                }

                if (IsIndexObstacle(x, y))
                {
                    if (fillobstacle)
                    {
                        visited[x, y] = true;
                        result.Area.Add(position);
                    }
                    continue;
                }

                visited[x, y] = true;
                result.Area.Add(position);

                // Enqueue neighboring positions
                queue.Enqueue(new Vector3Int(position.x, position.y + 1));
                queue.Enqueue(new Vector3Int(position.x, position.y - 1));
                queue.Enqueue(new Vector3Int(position.x + 1, position.y));
                queue.Enqueue(new Vector3Int(position.x - 1, position.y));
                
                if (filldiagonal)
                {
                    queue.Enqueue(new Vector3Int(position.x + 1, position.y + 1));
                    queue.Enqueue(new Vector3Int(position.x + 1, position.y - 1));
                    queue.Enqueue(new Vector3Int(position.x - 1, position.y + 1));
                    queue.Enqueue(new Vector3Int(position.x - 1, position.y - 1));
                }
            }
        }

        public bool IsPositionOutOfBounds(int x, int y)
        {
            return IsIndexOutOfBounds(x + offsetX, y + offsetY);
        }

        public bool HasVisitedPosition(int x, int y)
        {
            return HasVisitedIndex(x + offsetX, y + offsetY);
        }

        public bool IsPositionObstacle(int x, int y)
        {
            return IsIndexObstacle(x + offsetX, y + offsetY);
        }

        public bool IsIndexOutOfBounds(int x, int y)
        {
            return x < 0 || y < 0 || x > maxX || y > maxY;
        }

        public bool HasVisitedIndex(int x, int y)
        {
            return visited[x, y];
        }

        public bool IsIndexObstacle(int x, int y)
        {
            return obstacles[x, y];
        }
    }
}