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

        public FloodFill(Tilemap tilemap, BoundsInt bounds, FloodFillMode mode, bool fillDiagonal, Func<Tilemap, Vector3Int, bool> evaluator) : this(bounds, mode, fillDiagonal, pos => evaluator(tilemap, pos))
        {
            
        }

        public FloodFill(BoundsInt bounds, FloodFillMode mode, bool fillDiagonal, Func<Vector3Int, bool> evaluator)
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
                bool hasTile = evaluator(position);
                obstacles[x, y] = mode == FloodFillMode.Obstacle ? hasTile : !hasTile;
            }
        }

        public bool TryFill(Vector3Int startPosition, FloodFillResult result)
        {
            if (IsPositionOutOfBounds(startPosition) || 
                HasVisitedPosition(startPosition) || 
                IsPositionObstacle(startPosition))
            {
                return false;
            }

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

                if (HasVisitedIndex(x, y) || IsIndexObstacle(x, y))
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

            return true;
        }

        private bool IsPositionOutOfBounds(Vector3Int pos)
        {
            return IsIndexOutOfBounds(pos.x + offsetX, pos.y + offsetY);
        }

        private bool HasVisitedPosition(Vector3Int pos)
        {
            return HasVisitedIndex(pos.x + offsetX, pos.y + offsetY);
        }

        private bool IsPositionObstacle(Vector3Int pos)
        {
            return IsIndexObstacle(pos.x + offsetX, pos.y + offsetY);
        }

        private bool IsIndexOutOfBounds(int x, int y)
        {
            return x < 0 || y < 0 || x > maxX || y > maxY;
        }

        private bool HasVisitedIndex(int x, int y)
        {
            return visited[x, y];
        }

        private bool IsIndexObstacle(int x, int y)
        {
            return obstacles[x, y];
        }
    }
}