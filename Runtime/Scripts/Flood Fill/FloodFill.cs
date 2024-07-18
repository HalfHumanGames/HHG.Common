using System;
using System.Collections.Generic;
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

        private Queue<(int x, int y)> queue;
        private Dictionary<(int x, int y), (int x, int y)> parents;
        private BoundsData<int> bounds;
        private FloodFillResultBase result;
        private FloodFillResult fill => result as FloodFillResult;
        private FloodFillSearchResult search => result as FloodFillSearchResult;
        private FloodFillMode mode;
        private FloodFillOperation op;
        private bool isFill => op == FloodFillOperation.Fill;
        private bool isSearch => op == FloodFillOperation.Search;
        private (int x, int y)[] offsets;
        private (int x, int y) start;
        private bool[,] visited;
        private int mask;
        private float distance = -1;
        private Func<Vector3Int, bool> found;

        public FloodFill(BoundsData<int> boundsData, FloodFillMode fillMode, bool fillDiagonal, params int[] layers) : this(boundsData, fillMode, fillDiagonal, layers.ToFlags())
        {

        }

        public FloodFill(BoundsData<int> boundsData, FloodFillMode fillMode, bool fillDiagonal, int tileMask)
        {
            queue = new Queue<(int x, int y)>();
            parents = new Dictionary<(int x, int y), (int x, int y)>();
            bounds = boundsData;
            mode = fillMode;
            offsets = fillDiagonal ? offsets8 : offsets4;
            mask = tileMask;
            visited = new bool[bounds.Width, bounds.Height];
        }

        public bool TryFill(Vector3Int start, FloodFillResult fillResult)
        {
            op = FloodFillOperation.Fill;
            distance = -1;
            result = fillResult;
            return TryFillSearchInternal(start);
        }

        public bool TryFill(Vector3Int start, float maxDistance, FloodFillResult fillResult)
        {
            op = FloodFillOperation.Fill;
            distance = maxDistance;
            result = fillResult;
            return TryFillSearchInternal(start);
        }

        public bool TrySearch(Vector3Int start, Func<Vector3Int, bool> searchFunc, FloodFillSearchResult fillResult)
        {
            op = FloodFillOperation.Search;
            found = searchFunc;
            distance = -1;
            result = fillResult;
            ClearVisited();
            return TryFillSearchInternal(start);
        }

        public bool TrySearch(Vector3Int start, Func<Vector3Int, bool> searchFunc, float maxDistance, FloodFillSearchResult fillResult)
        {
            op = FloodFillOperation.Search;
            found = searchFunc;
            distance = maxDistance;
            result = fillResult;
            ClearVisited();
            return TryFillSearchInternal(start);
        }

        public bool TrySearch(Vector3Int start, IEnumerable<Vector3Int> targets, FloodFillSearchResult fillResult)
        {
            op = FloodFillOperation.Search;
            found = p => targets.Contains(p);
            distance = -1;
            result = fillResult;
            ClearVisited();
            return TryFillSearchInternal(start);
        }

        public bool TrySearch(Vector3Int start, IEnumerable<Vector3Int> targets, float maxDistance, FloodFillSearchResult fillResult)
        {
            op = FloodFillOperation.Search;
            found = p => targets.Contains(p);
            distance = maxDistance;
            result = fillResult;
            ClearVisited();
            return TryFillSearchInternal(start);
        }

        public void ConstructPath(Vector3Int from, Vector3Int to, List<Vector3Int> path)
        {
            ConstructPath(bounds.GetIndex(from), bounds.GetIndex(to), path);
        }

        private int GetPathLength((int x, int y) from, (int x, int y) to)
        {
            int length = 1;
            (int x, int y) current = to;
            while (current != from)
            {
                length++;
                current = parents[current];
            }
            return length;
        }

        private bool TryFillSearchInternal(Vector3Int position)
        {
            start = bounds.GetIndex(position);

            if (!IsValidStartIndex(start))
            {
                return false;
            }

            result.Reset();
            parents.Clear();
            queue.Clear();
            queue.Enqueue(start);
            visited[start.x, start.y] = true;

            if (isSearch && TryCompleteSearch(start))
            {
                return true;
            }

            while (queue.Count > 0)
            {
                (int x, int y) current = queue.Dequeue();

                if (isFill)
                {
                    fill.Area.Add(bounds.GetPosition(current));
                }

                EnqueueAdjacentPositions(current);

                if (isSearch && search.IsSuccess)
                {
                    return true;
                }
            }

            return isFill;
        }

        private bool HasVisitedIndex((int x, int y) pos)
        {
            return visited[pos.x, pos.y];
        }

        private bool IsIndexObstacle((int x, int y) pos)
        {
            int flags = bounds.GetData(pos);
            bool match = (flags & mask) != 0;
            return mode == FloodFillMode.Obstacle ? match : !match;
        }

        private void ClearVisited()
        {
            Array.Clear(visited, 0, visited.Length);
        }

        private bool IsValidStartIndex((int x, int y) pos)
        {
            return !bounds.IsOutOfBounds(pos) && !HasVisitedIndex(pos) && (isSearch || !IsIndexObstacle(pos));
        }

        private bool FoundSearchTarget((int x, int y) pos, out Vector3Int target)
        {
            target = bounds.GetPosition(pos);
            return found.Invoke(target);
        }

        private bool SurpassedMaxDistance(float currentDistance)
        {
            return distance > 0 && currentDistance >= distance;
        }

        private float GetDistance((int x, int y) pos)
        {
            return distance > 0f ? Mathf.Sqrt(Mathf.Pow(pos.x - start.x, 2f) + Mathf.Pow(pos.y - start.y, 2f)) : distance;
        }

        private void EnqueueAdjacentPositions((int x, int y) pos)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                (int x, int y) adjacent = (pos.x + offsets[i].x, pos.y + offsets[i].y);

                if (CanEnqueuePosition(adjacent))
                {
                    if (isSearch)
                    {
                        parents[adjacent] = pos;

                        if (TryCompleteSearch(pos))
                        {
                            return;
                        }
                    }

                    queue.Enqueue(adjacent);
                    visited[adjacent.x, adjacent.y] = true;
                }
            }
        }

        private bool CanEnqueuePosition((int x, int y) adj)
        {
            bool outOfBounds = bounds.IsOutOfBounds(adj);

            if (isFill && outOfBounds)
            {
                fill.AreaBordersEdge = true;
            }

            return !outOfBounds && !visited[adj.x, adj.y] && (isSearch || !IsIndexObstacle(adj)) && !SurpassedMaxDistance(GetDistance(adj));
        }

        private bool TryCompleteSearch((int x, int y) pos)
        {
            if (FoundSearchTarget(pos, out Vector3Int target))
            {
                search.IsSuccess = true;
                search.TargetPosition = target;
                search.Distance = GetDistance(pos);
                ConstructPath(start, pos, search.Path);
                return true;
            }
            return false;
        }


        public int GetPathLength(Vector3Int from, Vector3Int to)
        {
            return GetPathLength(bounds.GetIndex(from), bounds.GetIndex(to));
        }

        private void ConstructPath((int x, int y) from, (int x, int y) to, List<Vector3Int> path)
        {
            path.Clear();
            (int x, int y) current = to;
            while (current != from)
            {
                path.Add(bounds.GetPosition(current));
                current = parents[current];
            }
            path.Add(bounds.GetPosition(from));
            path.Reverse();
        }
    }
}