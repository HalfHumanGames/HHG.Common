namespace HHG.Common.Runtime
{
    public interface ILayerdTile
    {
        public int TileLayer { get; }
    }

    public static class ITileLayerExtensions
    {
        public static bool HasTileLayer(this ILayerdTile tile, int tileLayer)
        {
            return tile.TileLayer == tileLayer;
        }

        public static bool HasTileLayer(this ILayerdTile tile, TileLayerAsset tileLayer)
        {
            return tile.TileLayer == tileLayer;
        }
    }
}