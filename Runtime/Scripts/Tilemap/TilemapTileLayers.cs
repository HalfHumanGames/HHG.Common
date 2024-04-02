using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(Tilemap))]
    public class TilemapTileLayers : MonoBehaviour
    {
        private int offsetX;
        private int offsetY;
        private int maxX;
        private int maxY;
        private int[,] layers = new int[0, 0];

        private Tilemap tilemap;

        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();
            Initialize(tilemap.cellBounds);
            Tilemap.tilemapTileChanged += OnTileChanged;
        }

        private void OnTileChanged(Tilemap map, Tilemap.SyncTile[] syncs)
        {
            if (tilemap == map)
            {
                foreach (Tilemap.SyncTile sync in syncs)
                {
                    SetTileLayer(sync.position, sync.tile is ILayerdTile tile ? tile.TileLayer : 0);
                }
            }
        }

        public int this[Vector3Int pos]
        {
            get => GetTileLayer(pos);
            set => SetTileLayer(pos, value);
        }

        public void Initialize(BoundsInt bounds)
        {
            int width = bounds.size.x;
            int height = bounds.size.y;
            offsetX = -bounds.min.x;
            offsetY = -bounds.min.y;
            maxX = width - 1;
            maxY = height - 1;
            layers = new int[width, height];
        }

        public bool TryGetTileLayer(Vector3Int position, out int layer)
        {
            (int x, int y) = GetIndex(position);
            if (IsInBounds(x, y))
            {
                layer = layers[x, y];
                return true;
            }
            else
            {
                layer = default;
                return false;
            }
        }

        public bool HasTileLayer(Vector3Int position, int layer)
        {
            return GetTileLayer(position) == layer;
        }

        public int GetTileLayer(Vector3Int position)
        {
            if (TryGetTileLayer(position, out int layer))
            {
                return layer;
            }

            return default;
        }

        public void SetTileLayer(Vector3Int position, int value)
        {
            (int x, int y) = GetIndex(position);
            if (IsInBounds(x, y))
            {
                layers[x, y] = value;
            }
        }

        private (int x, int y) GetIndex(Vector3Int position)
        {
            return (position.x + offsetX, position.y + offsetY);
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x <= maxX && y <= maxY;
        }

        private void OnDestroy()
        {
            Tilemap.tilemapTileChanged += OnTileChanged;
        }
    }
}