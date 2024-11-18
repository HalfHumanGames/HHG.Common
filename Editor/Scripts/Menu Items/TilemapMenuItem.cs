using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    public static class TilemapMenuItem
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