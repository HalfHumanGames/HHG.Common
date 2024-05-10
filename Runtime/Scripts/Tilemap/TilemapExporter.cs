using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    public class TilemapExporter : AssetExporterBase<TilemapAsset>
    {
        [SerializeField] private Tilemap[] tilemaps = new Tilemap[0];

        protected override void SaveAsset(TilemapAsset asset)
        {
            asset?.Serialize(tilemaps);
        }

        protected override void LoadAsset(TilemapAsset asset)
        {
            asset?.Deserialize(tilemaps);

            // Fixes weird bug where colliders don't work
            transform.root.gameObject.SetActive(false);
            transform.root.gameObject.SetActive(true);
        }

        protected override void ClearAsset(TilemapAsset asset)
        {
            asset?.Clear();

            foreach (Tilemap tilemap in tilemaps)
            {
                tilemap.ClearAllTiles();
            }
        }
    }
}