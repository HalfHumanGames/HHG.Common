using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(Grid))]
    public class GridTileLayerMasks : MonoBehaviour
    {
        public BoundsData<int> Data => data;

        [SerializeField] private Tilemap[] tilemaps;

        private BoundsData<int> data;
        
        private void Awake()
        {
            Initialize();
            Tilemap.tilemapTileChanged += OnTileChanged;
        }

        private void OnTileChanged(Tilemap map, Tilemap.SyncTile[] syncs)
        {
            if (tilemaps.Contains(map))
            {
                foreach (Tilemap.SyncTile sync in syncs)
                {
                    int mask = 0;
                    foreach (Tilemap tilemap in tilemaps)
                    {
                        mask |= 1 << (tilemap.GetTile(sync.position) is ILayerdTile tile ? tile.TileLayer : 0);
                    }
                    SetTileLayerMask(sync.position, mask);
                }
            }
        }

        public int this[Vector3Int pos]
        {
            get => data[pos];
            set => data[pos] = value;
        }

        public void Initialize()
        {
            BoundsInt bounds = new BoundsInt();
            foreach (Tilemap tilemap in tilemaps)
            {
                bounds = bounds.Encapsulated(tilemap.cellBounds);
            }
            Initialize(bounds);
        }

        public void Initialize(BoundsInt bounds)
        {
            data = new BoundsData<int>(bounds);
        }

        public bool TryGetTileLayerMask(Vector3Int position, out int mask)
        {
            return data.TryGetData(position, out mask);
        }

        public bool HasTileLayerMask(Vector3Int position, int mask)
        {
            return data.DataEquals(position, mask);
        }

        public bool HasTileLayer(Vector3Int position, int layer)
        {
            return data.GetData(position).HasFlag(layer);
        }

        public int GetTileLayerMask(Vector3Int position)
        {
            return data.GetData(position);
        }

        public void SetTileLayerMask(Vector3Int position, int mask)
        {
            data.SetData(position, mask);
        }

        public void AddTileLayer(Vector3Int position, int layer)
        {
            data.SetData(position, data.GetData(position).WithFlag(layer));
        }

        public void RemoveTileLayer(Vector3Int position, int layer)
        {
            data.SetData(position, data.GetData(position).WithoutFlag(layer));
        }

        private void OnDestroy()
        {
            Tilemap.tilemapTileChanged += OnTileChanged;
        }
    }
}