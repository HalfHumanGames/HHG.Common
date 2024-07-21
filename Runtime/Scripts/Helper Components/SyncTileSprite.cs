using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    public class SyncTileSprite : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Tilemap tilemap;
        private Vector3Int position;

        private void Start()
        {
            // Initialize in Start since not added to tilemap in Awake
            spriteRenderer = GetComponent<SpriteRenderer>();
            tilemap = GetComponentInParent<Tilemap>(true);
            position = tilemap.WorldToCell(transform.position);
            spriteRenderer.sprite = tilemap.GetSprite(position);

            Tilemap.tilemapTileChanged += OnTileChanged;
        }

        private void OnTileChanged(Tilemap map, Tilemap.SyncTile[] syncTiles)
        {
            if (map == tilemap)
            {
                spriteRenderer.sprite = tilemap.GetSprite(position);
            }
        }

        private void OnDestroy()
        {
            Tilemap.tilemapTileChanged -= OnTileChanged;
        }
    }
}