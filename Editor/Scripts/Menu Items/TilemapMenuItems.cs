using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Editor
{
    public static class TilemapMenuItems
    {
        [MenuItem("CONTEXT/Tilemap/Print Tilemap Bounds")]
        public static void PrintTilemapBounds()
        {
            if (Selection.activeGameObject.TryGetComponent(out Tilemap tilemap))
            {
                tilemap.CompressBounds();
                Debug.Log(tilemap.cellBounds);
            }
        }
    }
}