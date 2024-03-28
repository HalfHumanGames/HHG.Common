using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    [CreateAssetMenu(fileName = "Tilemap", menuName = "HHG/Assets/Tilemap")]
    public class TilemapAsset : ScriptableObject
    {
        [SerializeField] private List<TileBase> tiles = new List<TileBase>();
        [SerializeField] private List<SerializableTilemap> serialized = new List<SerializableTilemap>();

        public void Serialize(params Tilemap[] tilemaps)
        {
            tiles.Clear();
            serialized.Clear();

            foreach (Tilemap tilemap in tilemaps)
            {
                serialized.Add(new SerializableTilemap(tilemap, tiles));
            }
        }

        public void Deserialize(params Tilemap[] tilemaps)
        {
            foreach (SerializableTilemap tilemap in serialized)
            {
                for (int i = 0; i < tilemaps.Length; i++)
                {
                    if (tilemap.Name == tilemaps[i].name)
                    {
                        tilemap.Deserialize(tilemaps[i], tiles);
                        break;
                    }
                }
            }
        }
    }
}