using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    public class TilemapExporter : AssetExporterBase<TilemapAsset>
    {
        [SerializeField] private Tilemap[] tilemaps = new Tilemap[0];

        public override void Save(TilemapAsset asset)
        {
            asset?.Serialize(tilemaps);
        }

        public override void Load(TilemapAsset asset)
        {
            asset?.Deserialize(tilemaps);

            // Fixes weird bug where colliders don't work
            transform.root.gameObject.SetActive(false);
            transform.root.gameObject.SetActive(true);
        }

        public override void Clear(TilemapAsset asset)
        {
            foreach (Tilemap tilemap in tilemaps)
            {
                tilemap.ClearAllTiles();
            }
        }
    }
}