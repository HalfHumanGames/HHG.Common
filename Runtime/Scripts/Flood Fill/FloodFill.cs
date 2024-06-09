using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class FloodFill
    {
        private readonly (int x, int y)[] offsets4 = {
            (0, 1),
            (0, -1),
            (1, 0),
            (-1, 0)
        };

        private readonly (int x, int y)[] offsets8 = {
            (0, 1),
            (0, -1),
            (1, 0),
            (-1, 0),
            (1, 1),
            (1, -1),
            (-1, 1),
            (-1, -1)
        };

        private Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
        private Dictionary<(int x, int y), (int x, int y)> parents = new Dictionary<(int x, int y), (int x, int y)>();
        private BoundsData<int> bounds;
        private FloodFillMode mode;
        private bool[,] visited;
        private bool diagonal;
        private int mask;
        private Func<Vector3Int, bool> found;

        public FloodFill(BoundsData<int> boundsData, FloodFillMode fillMode, bool fillDiagonal, params int[] layers) : this(boundsData, fillMode, fillDiagonal, layers.ToFlags())
        {

        }

        public FloodFill(BoundsData<int> boundsData, FloodFillMode fillMode, bool fillDiagonal, int tileMask)
        {
            bounds = boundsData;
            mode = fillMode;
            diagonal = fillDiagonal;
            mask = tileMask;
            visited = new bool[bounds.Width, bounds.Height];
        }

        public bool TryFill(Vector3Int start, FloodFillResult result)
        {
            return TryFillSearchInternal(start, -1, result);
        }

        public bool TryFill(Vector3Int start, float maxDistance, FloodFillResult result)
        {
            return TryFillSearchInternal(start, maxDistance, result);
        }

        public bool TrySearch(Vector3Int start, Func<Vector3Int, bool> searchFunc, FloodFillSearchResult result)
        {
            found = searchFunc;
            ClearVisited();
            return TryFillSearchInternal(start, -1, result);
        }

        public bool TrySearch(Vector3Int start, Func<Vector3Int, bool> searchFunc, float maxDistance, FloodFillSearchResult result)
        {
            found = searchFunc;
            ClearVisited();
            return TryFillSearchInternal(start, maxDistance, result);
        }

        public bool TrySearch(Vector3Int start, IEnumerable<Vector3Int> targets, FloodFillSearchResult result)
        {
            found = p => targets.Contains(p);
            ClearVisited();
            return TryFillSearchInternal(start, -1, result);
        }

        public bool TrySearch(Vector3Int start, IEnumerable<Vector3Int> targets, float maxDistance, FloodFillSearchResult result)
        {
            found = p => targets.Contains(p);
            ClearVisited();
            return TryFillSearchInternal(start, maxDistance, result);
        }

        private bool TryFillSearchInternal(Vector3Int position, float maxDistance, FloodFillResultBase result)
        {
            (int x, int y) start = bounds.GetIndex(position);

            FloodFillResult fill = result as FloodFillResult;
            FloodFillSearchResult search = result as FloodFillSearchResult;

            if (!IsValidStartIndex(start, fill))
            {
                return false;
            }

            result.Reset();
            parents.Clear();
            queue.Clear();
            queue.Enqueue(start);

            (int x, int y)[] offsets = diagonal ? offsets8 : offsets4;

            bool forceEnqueue = search != null;
            while (queue.Count > 0)
            {
                (int x, int y) current = queue.Dequeue();

                if (FoundSearchTarget(start, current, search))
                {
                    return true;
                }

                if (forceEnqueue || TryVisitIndex(current, fill))
                {
                    forceEnqueue = false;
                    EnqueueAdjacentPositions(start, current, offsets, maxDistance, search);
                }
            }

            return fill != null;
        }

        private bool HasVisitedIndex((int x, int y) pos)
        {
            return visited[pos.x, pos.y];
        }

        private bool IsIndexObstacle((int x, int y) pos)
        {
            int flags = bounds.GetData(pos);
            bool match = (flags & mask) != 0;
            return  mode == FloodFillMode.Obstacle ? match : !match;
        }

        private void ClearVisited()
        {
            for (int i = 0; i < bounds.Width; i++)
            {
                for (int j = 0; j < bounds.Height; j++)
                {
                    visited[i, j] = false;
                }
            }
        }

        private bool IsValidStartIndex((int x, int y) pos, FloodFillResult fill)
        {
            return !bounds.IsOutOfBounds(pos) && !HasVisitedIndex(pos) && (fill == null || !IsIndexObstacle(pos));
        }

        private bool FoundSearchTarget((int x, int y) start, (int x, int y) pos, FloodFillSearchResult search)
        {
            if (search != null)
            {
                Vector3Int vec = bounds.GetPosition(pos);
                if (found.Invoke(vec))
                {
                    search.IsSuccess = true;
                    search.TargetPosition = vec;
                    search.Distance = GetDistance(start, pos);
                    ConstructPath(start, pos, search.Path);
                    return true;
                }
            }
            return false;
        }

        private bool SurpassedMaxDistance(float currentDistance, float maxDistance)
        {
            return maxDistance > 0 && currentDistance >= maxDistance;
        }

        private float GetDistance((int x, int y) start, (int x, int y) pos)
        {
            return Mathf.Sqrt(Mathf.Pow(pos.x - start.x, 2) + Mathf.Pow(pos.y - start.y, 2));
        }

        private bool TryVisitIndex((int x, int y) pos, FloodFillResult fill)
        {  
            if (bounds.IsOutOfBounds(pos))
            {
                if (fill != null)
                {
                    fill.AreaBordersEdge = true;
                }
                return false;
            }

            if (HasVisitedIndex(pos) || IsIndexObstacle(pos))
            {
                return false;
            }

            fill?.Area.Add(bounds.GetPosition(pos));
            visited[pos.x, pos.y] = true;
            return true;
        }

        private void EnqueueAdjacentPositions((int x, int y) start, (int x, int y) current, (int x, int y)[] offsets, float maxDistance, FloodFillSearchResult search)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                (int x, int y) adjacent = (current.x + offsets[i].x, current.y + offsets[i].y);

                if (!SurpassedMaxDistance(GetDistance(start, current), maxDistance))
                {
                    if (search != null && !parents.ContainsKey(adjacent))
                    {
                        parents[adjacent] = current;
                    }

                    queue.Enqueue(adjacent);
                }
            }
        }

        public int GetPathLength(Vector3Int start, Vector3Int target)
        {
            int len = GetPathLength(bounds.GetIndex(start), bounds.GetIndex(target));
            return len;
        }

        private int GetPathLength((int x, int y) start, (int x, int y) target)
        {
            int length = 1;
            (int x, int y) current = target;
            while (current != start)
            {
                length++;
                current = parents[current];
            }
            return length;
        }

        public void ConstructPath(Vector3Int start, Vector3Int target, List<Vector3Int> path)
        {
            ConstructPath(bounds.GetIndex(start), bounds.GetIndex(target), path);
        }

        private void ConstructPath((int x, int y) start, (int x, int y) target, List<Vector3Int> path)
        {
            path.Clear();
            (int x, int y) current = target;
            while (current != start)
            {
                path.Add(bounds.GetPosition(current));
                current = parents[current];
            }
            path.Add(bounds.GetPosition(start));
            path.Reverse();
        }
    }
}