using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    [CreateAssetMenu(fileName = "Linked Tile Map", menuName = "HHG/Assets/Tilemap/Linked Tile Map")]
    public class LinkedTileMapAsset : ScriptableObject
    {
        public IReadOnlyDictionary<TileBase, TileBase> Map => map;

        [SerializeField] private SerializableDictionary<TileBase, TileBase> map = new SerializableDictionary<TileBase, TileBase>();
    }
}