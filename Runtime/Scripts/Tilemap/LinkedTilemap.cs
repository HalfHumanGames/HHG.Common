using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    public class LinkedTilemap : MonoBehaviour
    {
        [SerializeField] private Tilemap sourceTilemap;
        [SerializeField] private Tilemap targetTilemap;
        [SerializeField, FormerlySerializedAs("linkedTileMap")] private TileLookupMapAsset tileLookupMap;

        private bool initialized;

        private void Awake()
        {
            TryInitialize();
        }

        private void OnEnable()
        {
            Tilemap.tilemapTileChanged += OnTileChanged;
        }

        private void OnDisable()
        {
            Tilemap.tilemapTileChanged -= OnTileChanged;
        }

        private void OnTileChanged(Tilemap map, Tilemap.SyncTile[] syncs)
        {
            if (sourceTilemap == map)
            {
                foreach (Tilemap.SyncTile sync in syncs)
                {
                   UpdatedLinkedTile(sync.tile, sync.position);
                }
            }
        }

        private void UpdatedLinkedTile(TileBase tile, Vector3Int position)
        {
            targetTilemap.SetTile(position, tile == null ? null : tileLookupMap.Map.TryGetValue(tile, out TileBase set) ? set : tile);
        }

        public void TryInitialize()
        {
            if (!initialized)
            {
                ForceInitialize();
            }
        }

        public void ForceInitialize()
        {
            targetTilemap.ClearAllTiles();

            foreach (Vector3Int position in sourceTilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = sourceTilemap.GetTile(position);
                UpdatedLinkedTile(tile, position);
            }
        }
    }
}