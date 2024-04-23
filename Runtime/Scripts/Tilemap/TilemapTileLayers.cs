using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(Tilemap))]
    public class TilemapTileLayers : MonoBehaviour
    {
        public BoundsData<int> Data => data;
        private BoundsData<int> data;
        private Tilemap tilemap;

        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();

            if (data == null)
            {
                Initialize(tilemap.cellBounds);
            }

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
            get => data[pos];
            set => data[pos] = value;
        }

        public void Initialize(BoundsInt bounds, bool search = false)
        {
            data = new BoundsData<int>(bounds);

            if (search)
            {
                foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
                {
                    SetTileLayer(position, tilemap.GetTile(position) is ILayerdTile tile ? tile.TileLayer : 0);
                }
            }
        }

        public bool TryGetTileLayer(Vector3Int position, out int layer)
        {
            return data.TryGetData(position, out layer);
        }

        public bool HasTileLayer(Vector3Int position, int layer)
        {
            return data.DataEquals(position, layer);
        }

        public int GetTileLayer(Vector3Int position)
        {
            return data.GetData(position);
        }

        public void SetTileLayer(Vector3Int position, int value)
        {
            data.SetData(position, value);
        }

        private void OnDestroy()
        {
            Tilemap.tilemapTileChanged -= OnTileChanged;
        }
    }
}