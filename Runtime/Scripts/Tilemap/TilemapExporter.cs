using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common
{
    public class TilemapExporter : AssetExporterBase<TilemapAsset>
    {
        [SerializeField] private Tilemap[] tilemaps = new Tilemap[0];

        protected override void Save(TilemapAsset asset)
        {
            asset?.Serialize(tilemaps);
        }

        protected override void Load(TilemapAsset asset)
        {
            asset?.Deserialize(tilemaps);
        }

        protected override void Clear(TilemapAsset asset)
        {
            foreach (Tilemap tilemap in tilemaps)
            {
                tilemap.ClearAllTiles();
            }
        }
    }
}