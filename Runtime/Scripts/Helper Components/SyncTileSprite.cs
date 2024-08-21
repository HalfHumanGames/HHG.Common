using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    public class SyncTileSprite : MonoBehaviour
    {
        [SerializeField] private bool useLookupMap;
        [SerializeField] private SpriteLookupMapAsset lookupMap;

        private SpriteRenderer spriteRenderer;
        private Tilemap tilemap;
        private Vector3Int position;
        private bool hasStarted;

        private void Start()
        {
            // Initialize in Start since not added to tilemap in Awake
            // Transform gets parented to tilemap between Awake and Start
            tilemap = GetComponentInParent<Tilemap>(true);
            position = tilemap.WorldToCell(transform.position);
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = tilemap.GetSprite(position);
            
            SyncSprite(); // Since not called in first OnEnable

            hasStarted = true;
        }

        private void OnEnable()
        {
            if (hasStarted)
            {
                SyncSprite();
            }

            Tilemap.tilemapTileChanged += OnTileChanged;
        }

        private void OnDisable()
        {
            Tilemap.tilemapTileChanged -= OnTileChanged;
        }

        private void OnTileChanged(Tilemap map, Tilemap.SyncTile[] syncTiles)
        {
            if (map == tilemap)
            {
                SyncSprite();
            }
        }

        private void SyncSprite()
        {
            Sprite sprite = tilemap.GetSprite(position);

            if (useLookupMap && lookupMap && lookupMap.Map.TryGetValue(sprite, out sprite))
            {
                spriteRenderer.sprite = sprite;
            }
            else
            {
                spriteRenderer.sprite = sprite;
            }
        }
    }
}