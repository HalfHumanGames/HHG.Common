using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class FloodFill
    {
        private readonly Vector3Int[] offsets4Directions = {
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0)
        };

        private readonly Vector3Int[] offsets8Directions = {
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, -1, 0)
        };

        private Queue<Vector3Int> queue;
        private BoundsData<int> data;
        private FloodFillMode mode;
        private bool[,] visited;
        private bool diagonal;
        private int mask;
        private Func<Vector3Int, bool> search;

        public FloodFill(BoundsData<int> boundsData, FloodFillMode fillMode, bool fillDiagonal, params int[] layers) : this(boundsData, fillMode, fillDiagonal, layers.ToFlags())
        {

        }

        public FloodFill(BoundsData<int> boundsData, FloodFillMode fillMode, bool fillDiagonal, int tileMask)
        {
            queue = new Queue<Vector3Int>();
            data = boundsData;
            mode = fillMode;
            diagonal = fillDiagonal;
            mask = tileMask;
            visited = new bool[data.Width, data.Height];
        }

        public bool TryFill(Vector3Int startPosition, FloodFillResult result)
        {
            return TryFillSearchInternal(startPosition, result);
        }

        public bool TrySearch(Vector3Int startPosition, Func<Vector3Int, bool> searchFunc, FloodFillSearchResult result)
        {
            search = searchFunc;
            ClearVisited();
            return TryFillSearchInternal(startPosition, result);
        }

        public bool TrySearch(Vector3Int startPosition, IEnumerable<Vector3Int> targets, FloodFillSearchResult result)
        {
            search = p => targets.Contains(p);
            ClearVisited();
            return TryFillSearchInternal(startPosition, result);
        }

        private bool TryFillSearchInternal(Vector3Int startPosition, FloodFillResultBase result)
        {
            if (data.IsOutOfBounds(startPosition) ||
                HasVisitedPosition(startPosition) ||
                IsPositionObstacle(startPosition))
            {
                return false;
            }

            result.Reset();
            queue.Clear();
            queue.Enqueue(startPosition);

            FloodFillResult fillResult = result as FloodFillResult;
            FloodFillSearchResult searchResult = result as FloodFillSearchResult;

            Vector3Int[] offsets = diagonal ? offsets8Directions : offsets4Directions;

            do
            {
                Vector3Int position = queue.Dequeue();

                if (searchResult != null && search?.Invoke(position) is bool b && b)
                {
                    searchResult.IsSuccess = true;
                    searchResult.TargetPosition = position;
                    return true;
                }

                (int x, int y) = data.GetIndex(position);

                // Make sure to perform the out-of-bounds check first
                if (data.IsOutOfBounds(x, y))
                {
                    if (fillResult != null)
                    {
                        fillResult.AreaBordersEdge = true;
                    }
                    continue;
                }

                if (HasVisitedIndex(x, y) || IsIndexObstacle(x, y))
                {
                    continue;
                }

                visited[x, y] = true;
                fillResult?.Area.Add(position);

                for (int i = 0; i < offsets.Length; i++)
                {
                    Vector3Int adjacent = position + offsets[i];
                    queue.Enqueue(adjacent);
                }
            } while (queue.Count > 0);

            return fillResult != null;
        }

        private bool HasVisitedPosition(Vector3Int position)
        {
            (int x, int y) = data.GetIndex(position);
            return HasVisitedIndex(x, y);
        }

        private bool IsPositionObstacle(Vector3Int position)
        {
            (int x, int y) = data.GetIndex(position);
            return IsIndexObstacle(x, y);
        }

        private bool HasVisitedIndex(int x, int y)
        {
            return visited[x, y];
        }

        private bool IsIndexObstacle(int x, int y)
        {
            int flags = data.GetData(x, y);
            bool match = (flags & mask) != 0;
            return  mode == FloodFillMode.Obstacle ? match : !match;
        }

        private void ClearVisited()
        {
            for (int i = 0; i < data.Width; i++)
            {
                for (int j = 0; j < data.Height; j++)
                {
                    visited[i, j] = false;
                }
            }
        }
    }
}