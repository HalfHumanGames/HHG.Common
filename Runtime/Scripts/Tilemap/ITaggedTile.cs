using System.Linq;

namespace HHG.Common
{
    public interface ITaggedTile
    {
        public TileTagAsset[] TileTags { get; }
    }

    public static class ITaggedTileExtensions
    {
        public static bool HasTag(this ITaggedTile tile, params TileTagAsset[] tags)
        {
            if (tile == null || tags == null) return false;

            return tags.All(t => tile.TileTags.Contains(t));
        }

        public static bool HasTag(this ITaggedTile tile, params string[] tags)
        {
            if (tile == null || tags == null) return false;

            return tags.All(t => tile.TileTags.Any(t1 => t1.name == t));
        }
    }
}