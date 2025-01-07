using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.Common.Runtime
{
    [Serializable]
    public class SerializableTilemap
    {
        private const int emptyTileIndex = -1;

        public string Name => name;
        public BoundsInt Bounds => bounds;
        public string Encoded => encoded;

        [SerializeField] private string name;
        [SerializeField] private BoundsInt bounds;
        [SerializeField] private string encoded;

        private byte[] data;

        public SerializableTilemap(Tilemap tilemap, List<TileBase> usedTiles)
        {
            name = tilemap.name;

            tilemap.CompressBounds();

            bounds = tilemap.cellBounds;

            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

            data = new byte[allTiles.Length];

            for (int i = 0; i < allTiles.Length; i++)
            {
                TileBase tile = allTiles[i];

                if (tile != null && !usedTiles.Contains(tile))
                {
                    usedTiles.Add(tile);
                }

                data[i] = (byte) (data == null ? 0 : usedTiles.IndexOf(tile) + 1);
            }

            encoded = Convert.ToBase64String(GZipUtil.Compress(data));
        }

        public void Deserialize(Tilemap tilemap, List<TileBase> usedTiles)
        {
            tilemap.ClearAllTiles();

            LoadData();

            TileBase[] tileBases = new TileBase[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                int usedTileIndex = data[i] - 1;

                if (usedTileIndex != emptyTileIndex)
                {
                    tileBases[i] = usedTiles[usedTileIndex];
                }
            }

            tilemap.SetTilesBlock(bounds, tileBases);
        }

        public void LoadData()
        {
            data = GZipUtil.Decompress(Convert.FromBase64String(encoded));
        }

        public TileBase GetTile(Vector3Int position, IReadOnlyList<TileBase> usedTiles)
        {
            if (data == null)
            {
                LoadData();
            }

            int i = 0;
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                if (pos == position)
                {
                    int usedTileIndex = data[i++] - 1;
                    return usedTileIndex != emptyTileIndex ? usedTiles[usedTileIndex] : null;
                }
            }

            return null;
        }
    }
}