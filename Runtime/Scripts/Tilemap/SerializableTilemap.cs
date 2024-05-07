using System;
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

        public SerializableTilemap(Tilemap tilemap, List<TileBase> usedTiles)
        {
            name = tilemap.name;

            tilemap.CompressBounds();

            bounds = tilemap.cellBounds;

            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

            byte[] tiles = new byte[allTiles.Length];

            for (int i = 0; i < allTiles.Length; i++)
            {
                TileBase tile = allTiles[i];

                if (tile != null && !usedTiles.Contains(tile))
                {
                    usedTiles.Add(tile);
                }

                tiles[i] = (byte) (tiles == null ? 0 : usedTiles.IndexOf(tile) + 1);
            }

            encoded = Convert.ToBase64String(GZipUtil.Compress(tiles));
        }

        public void Deserialize(Tilemap tilemap, List<TileBase> usedTiles)
        {
            tilemap.ClearAllTiles();

            byte[] tiles = GZipUtil.Decompress(Convert.FromBase64String(encoded));

            TileBase[] tileBases = new TileBase[tiles.Length];

            for (int i = 0; i < tiles.Length; i++)
            {
                int usedTileIndex = tiles[i] - 1;

                if (usedTileIndex != emptyTileIndex)
                {
                    tileBases[i] = usedTiles[usedTileIndex];
                }
            }

            tilemap.SetTilesBlock(bounds, tileBases);
        }
    }
}