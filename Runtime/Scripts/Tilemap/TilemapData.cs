using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(Tilemap))]
    public class TilemapData : DataBase<Vector3Int, object>
    {
        private Tilemap tilemap;

        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();
        }

        protected override void OnSetData(Vector3Int position)
        {
            tilemap.RefreshTile(position);
        }
    }
}