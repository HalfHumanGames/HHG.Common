using UnityEngine.Tilemaps;

namespace HHG.Common
{
    public static class ITilemapExtensions
    {
        public static Tilemap GetTilemap(this ITilemap tilemap)
        {
            return tilemap.GetComponent<Tilemap>();
        }
    }
}