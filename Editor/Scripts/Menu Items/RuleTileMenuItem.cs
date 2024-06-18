using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    public static class RuleTileMenuItem
    {
        [MenuItem("CONTEXT/RuleTile/Duplicate")]
        private static void Duplicate()
        {
            foreach (RuleTile tile in Selection.objects.OfType<RuleTile>())
            {
                string tilePath = AssetDatabase.GetAssetPath(tile);
                string tileDir = System.IO.Path.GetDirectoryName(tilePath);
                string tileName = System.IO.Path.GetFileNameWithoutExtension(tilePath);
                string newTilePath = AssetDatabase.GenerateUniqueAssetPath(tileDir + "/" + tileName + "_Copy.asset");

                RuleTile newTile = Object.Instantiate(tile);
                AssetDatabase.CreateAsset(newTile, newTilePath);

                var sprites = tile.m_TilingRules.SelectMany(rule => rule.m_Sprites).Distinct().ToArray();
                if (sprites.Length > 0)
                {
                    string spritePath = AssetDatabase.GetAssetPath(sprites[0]);
                    string spriteDir = System.IO.Path.GetDirectoryName(spritePath);
                    string spriteName = System.IO.Path.GetFileNameWithoutExtension(spritePath);
                    string spriteExtension = System.IO.Path.GetExtension(spritePath);
                    string newSpritePath = AssetDatabase.GenerateUniqueAssetPath(spriteDir + "/" + spriteName + "_Copy" + spriteExtension);

                    AssetDatabase.CopyAsset(spritePath, newSpritePath);
                    AssetDatabase.ImportAsset(newSpritePath, ImportAssetOptions.ForceUpdate);
                    Texture2D newTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(newSpritePath);
                    Sprite[] newSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(newSpritePath).OfType<Sprite>().ToArray();

                    Dictionary<Sprite, Sprite> spriteMap = sprites.
                                            Select((originalSprite, index) => new { 
                                                originalSprite, 
                                                newSprite = newSprites.FirstOrDefault(s => s.name == originalSprite.name)
                                            }).
                                            ToDictionary(pair => pair.originalSprite, pair => pair.newSprite);

                    newTile.m_DefaultSprite = spriteMap[newTile.m_DefaultSprite];

                    foreach (var rule in newTile.m_TilingRules)
                    {
                        for (int i = 0; i < rule.m_Sprites.Length; i++)
                        {
                            if (spriteMap.ContainsKey(rule.m_Sprites[i]))
                            {
                                rule.m_Sprites[i] = spriteMap[rule.m_Sprites[i]];
                            }
                        }
                    }

                    EditorUtility.SetDirty(newTile);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}