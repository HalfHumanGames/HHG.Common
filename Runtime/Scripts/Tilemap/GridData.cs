using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common
{
    [RequireComponent(typeof(Grid))]
    public class GridData : DataBase<Vector3Int, object>
    {
        private Tilemap[] tilemaps;

        private void Awake()
        {
            tilemaps = GetComponentsInChildren<Tilemap>();    
        }

        protected override void OnSetData(Vector3Int position)
        {
            foreach (Tilemap tilemap in tilemaps)
            {
                tilemap.RefreshTile(position);
            }
        }
    }
}