using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common
{
    public static class TilemapExtensions
    {
        public static bool HasAdjacentTile(this Tilemap tilemap, Vector3Int position, bool includeDiagonals = false, params TileTagAsset[] tags)
        {
            if (tags == null)
            {
                return false;
            }

            for (int i = 0; i < tags.Length; i++)
            {
                foreach (Vector3Int adj in position.GetAdjacentPositions(includeDiagonals))
                {
                    if (tilemap.GetTile(adj) is ITaggedTile taggedTile)
                    {
                        return taggedTile.HasTag(tags);
                    }
                }
            }

            return false;
        }

        public static bool HasAdjacentTile(this Tilemap tilemap, Vector3Int position, bool includeDiagonals = false, params string[] tags)
        {
            if (tags == null)
            {
                return false;
            }

            for (int i = 0; i < tags.Length; i++)
            {
                foreach (Vector3Int adj in position.GetAdjacentPositions(includeDiagonals))
                {
                    if (tilemap.GetTile(adj) is ITaggedTile taggedTile)
                    {
                        return taggedTile.HasTag(tags);
                    }
                }
            }

            return false;
        }

        public static bool HasAdjacentTile(this Tilemap tilemap, Vector3Int position, bool includeDiagonals = false, params TileBase[] tiles)
        {
            if (tiles == null)
            {
                tiles = new TileBase[1] { null };
            }


            foreach (Vector3Int adj in position.GetAdjacentPositions(includeDiagonals))
            {
                TileBase tile = tilemap.GetTile(adj);

                if (tiles.Length == 0 && tile != null)
                {
                    return true;
                }

                foreach (TileBase t in tiles)
                {
                    if (t == tile)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool HasTile(this Tilemap tilemap, Vector3Int position, params TileTagAsset[] tags)
        {
            if (tags == null)
            {
                return false;
            }

            for (int i = 0; i < tags.Length; i++)
            {
                if (tilemap.GetTile(position) is ITaggedTile taggedTile)
                {
                    return taggedTile.HasTag(tags);
                }
            }

            return false;
        }

        public static bool HasTile(this Tilemap tilemap, Vector3Int position, params string[] tags)
        {
            if (tags == null)
            {
                return false;
            }

            for (int i = 0; i < tags.Length; i++)
            {
                if (tilemap.GetTile(position) is ITaggedTile taggedTile)
                {
                    return taggedTile.HasTag(tags);
                }
            }

            return false;
        }

        public static bool HasTile(this Tilemap tilemap, Vector3Int position, params TileBase[] tiles)
        {
            if (tiles == null)
            {
                tiles = new TileBase[1] { null };
            }

            TileBase tile = tilemap.GetTile(position);

            if (tiles.Length == 0)
            {
                return tile != null;
            }

            foreach (TileBase t in tiles)
            {
                if (t == tile)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetComponentAtPosition<T>(this Tilemap tilemap, Vector3Int position, out T component)
        {
            component = tilemap.GetComponentAtPosition<T>(position);
            return component != null;
        }

        public static T GetComponentAtPosition<T>(this Tilemap tilemap, Vector3Int position)
        {
            return tilemap.GetInstantiatedObject(position) is GameObject obj ? obj.GetComponent<T>() : default;
        }

        public static T GetComponentInBounds<T>(this Tilemap tilemap, BoundsInt bounds)
        {
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                if (tilemap.GetComponentAtPosition<T>(pos) is T comp)
                {
                    return comp;
                }
            }

            return default;
        }

        public static bool TryGetComponentsAtPosition<T>(this Tilemap tilemap, Vector3Int position, out T[] components)
        {
            components = tilemap.GetComponentsAtPosition<T>(position);
            return components.Length > 0;
        }

        public static T[] GetComponentsAtPosition<T>(this Tilemap tilemap, Vector3Int position)
        {
            return tilemap.GetInstantiatedObject(position) is GameObject obj ? obj.GetComponentsInChildren<T>() : new T[0];
        }

        public static T[] GetComponentsInBounds<T>(this Tilemap tilemap, BoundsInt bounds)
        {
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                if (tilemap.TryGetComponentsAtPosition<T>(pos, out T[] components))
                {
                    return components;
                }
            }

            return new T[0];
        }

        public static bool ContainsPosition(this Tilemap tilemap, Vector3Int position)
        {
            return tilemap.cellBounds.Contains(position);
        }

        public static bool DoesPositionBorderEdge(this Tilemap tilemap, Vector3Int position)
        {
            Vector3Int min = tilemap.cellBounds.min;
            Vector3Int max = tilemap.cellBounds.max;

            return position.x == min.x || position.x == max.x ||
                   position.y == min.y || position.y == max.y;
        }

        public static int GetPositionIndex(this Tilemap tilemap, Vector3Int position)
        {
            Vector3Int min = tilemap.cellBounds.min;

            return (position.y + -min.y) * tilemap.size.x + position.x + -min.x;
        }

        public static int GetPositionCount(this Tilemap tilemap)
        {
            return tilemap.size.x * tilemap.size.y;
        }

        public static Vector3 WorldToCellWorld(this Tilemap tilemap, Vector3 position)
        {
            return tilemap.CellToWorld(tilemap.WorldToCell(position));
        }
    }
}