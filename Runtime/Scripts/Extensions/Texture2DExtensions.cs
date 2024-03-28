using UnityEngine;

namespace HHG.Common
{
    public static class Texture2DExtensions
    {
        public static Texture2D Clone(this Texture2D texture)
        {
            Texture2D cloned = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
            cloned.SetPixels(texture.GetPixels());
            cloned.Apply();
            return cloned;
        }
    }
}