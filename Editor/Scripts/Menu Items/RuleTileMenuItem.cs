using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Path = System.IO.Path;

namespace HHG.Common.Editor
{
    public static class RuleTileMenuItem
    {
        [MenuItem("CONTEXT/RuleTile/Duplicate")]
        private static void Duplicate(MenuCommand menuCommand)
        {
            if (!menuCommand.IsContextActiveObject())
            {
                return;
            }

            foreach (RuleTile tile in Selection.objects.OfType<RuleTile>())
            {
                foreach (var rule in tile.m_TilingRules)
                {
                    if (rule.m_Output == RuleTile.TilingRuleOutput.OutputSprite.Single)
                    {
                        Array.Resize(ref rule.m_Sprites, 1);
                    }
                }

                string tilePath = AssetDatabase.GetAssetPath(tile);
                string tileDir = Path.GetDirectoryName(tilePath);
                string tileName = Path.GetFileNameWithoutExtension(tilePath);
                string newTilePath = AssetDatabase.GenerateUniqueAssetPath(tileDir + "/" + tileName + "_Copy.asset");

                RuleTile newTile = Object.Instantiate(tile);
                AssetDatabase.CreateAsset(newTile, newTilePath);

                // Collect all distinct sprites from the tile
                var allSprites = tile.m_TilingRules.SelectMany(rule => rule.m_Sprites).Append(tile.m_DefaultSprite).Distinct().ToArray();
                var spritePaths = allSprites.Select(s => AssetDatabase.GetAssetPath(s)).Distinct();

                Dictionary<Sprite, Sprite> spriteMap = new Dictionary<Sprite, Sprite>();

                // Process each sprite sheet
                foreach (string spritePath in spritePaths)
                {
                    if (string.IsNullOrEmpty(spritePath))
                    {
                        continue;
                    }

                    string spriteDir = Path.GetDirectoryName(spritePath);
                    string spriteName = Path.GetFileNameWithoutExtension(spritePath);
                    string spriteExtension = Path.GetExtension(spritePath);
                    string spriteCopyPath = spriteDir + "/" + spriteName + "_Copy" + spriteExtension;

                    // Only create a new copy if it doesn't already exist yet
                    if (!File.Exists(spriteCopyPath))
                    {
                        AssetDatabase.CopyAsset(spritePath, spriteCopyPath);
                        AssetDatabase.ImportAsset(spriteCopyPath, ImportAssetOptions.ForceUpdate);
                    }

                    Sprite[] newSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(spriteCopyPath).OfType<Sprite>().ToArray();

                    // Map old sprites to new sprites
                    var originalSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(spritePath).OfType<Sprite>().ToArray();
                    for (int i = 0; i < originalSprites.Length; i++)
                    {
                        spriteMap[originalSprites[i]] = newSprites.FirstOrDefault(s => s.name == originalSprites[i].name);
                    }
                }

                // Update the new tile's default sprite and tiling rules with the new sprites
                newTile.m_DefaultSprite = spriteMap.ContainsKey(newTile.m_DefaultSprite) ? spriteMap[newTile.m_DefaultSprite] : newTile.m_DefaultSprite;

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
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
