using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

namespace HHG.Common.Editor
{
    public static class TMPSpriteAssetMenuItems
    {
        private const uint defaultUnicode = 0xFFFE;

        // Rebuilds the character and glyph tables from the sprites currently in the sprite sheet: one entry per
        // sprite name. Existing entries keep their unicode, metrics and scale; entries for missing sprites and
        // duplicate names are removed; new sprites are appended.
        [MenuItem("CONTEXT/TMP_SpriteAsset/Sync With Sprite Sheet")]
        private static void SyncWithSpriteSheet(MenuCommand menuCommand)
        {
            if (!menuCommand.IsContextActiveObject())
            {
                return;
            }

            foreach (TMP_SpriteAsset spriteAsset in Selection.objects.OfType<TMP_SpriteAsset>())
            {
                Sync(spriteAsset);
            }
        }

        private static void Sync(TMP_SpriteAsset spriteAsset)
        {
            string sheetPath = AssetDatabase.GetAssetPath(spriteAsset.spriteSheet);
            if (string.IsNullOrEmpty(sheetPath))
            {
                Debug.LogError($"{spriteAsset.name} has no sprite sheet.", spriteAsset);
                return;
            }

            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            foreach (Sprite sprite in AssetDatabase.LoadAllAssetsAtPath(sheetPath).OfType<Sprite>())
            {
                if (!sprites.TryAdd(sprite.name, sprite))
                {
                    Debug.LogWarning($"{sheetPath} has more than one sprite named '{sprite.name}'. Using the first.", spriteAsset);
                }
            }

            Undo.RecordObject(spriteAsset, "Sync With Sprite Sheet");

            List<TMP_SpriteCharacter> oldCharacters = spriteAsset.spriteCharacterTable.ToList();
            List<TMP_SpriteGlyph> oldGlyphs = spriteAsset.spriteGlyphTable.ToList();
            List<TMP_SpriteCharacter> characters = new List<TMP_SpriteCharacter>();
            List<TMP_SpriteGlyph> glyphs = new List<TMP_SpriteGlyph>();
            HashSet<string> synced = new HashSet<string>();
            int removed = 0;

            foreach (TMP_SpriteCharacter character in oldCharacters)
            {
                TMP_SpriteGlyph oldGlyph = oldGlyphs.FirstOrDefault(g => g.index == character.glyphIndex);
                if (oldGlyph == null || !sprites.TryGetValue(character.name, out Sprite sprite) || !synced.Add(character.name))
                {
                    removed++;
                    continue;
                }

                TMP_SpriteGlyph glyph = new TMP_SpriteGlyph((uint)glyphs.Count, oldGlyph.metrics, GetRect(sprite), oldGlyph.scale, oldGlyph.atlasIndex, sprite);
                characters.Add(CreateCharacter(character.unicode, character.name, character.scale, glyph));
                glyphs.Add(glyph);
            }

            int added = 0;
            foreach (Sprite sprite in sprites.Values.Where(s => !synced.Contains(s.name)).OrderBy(s => s.name))
            {
                TMP_SpriteGlyph glyph = new TMP_SpriteGlyph((uint)glyphs.Count, GetMetrics(sprite, oldGlyphs), GetRect(sprite), 1f, 0, sprite);
                characters.Add(CreateCharacter(defaultUnicode, sprite.name, 1f, glyph));
                glyphs.Add(glyph);
                added++;
            }

            spriteAsset.spriteCharacterTable.Clear();
            spriteAsset.spriteCharacterTable.AddRange(characters);
            spriteAsset.spriteGlyphTable.Clear();
            spriteAsset.spriteGlyphTable.AddRange(glyphs);
            spriteAsset.UpdateLookupTables();

            EditorUtility.SetDirty(spriteAsset);
            AssetDatabase.SaveAssetIfDirty(spriteAsset);
            Debug.Log($"{spriteAsset.name}: {characters.Count} sprites ({added} added, {removed} removed).", spriteAsset);
        }

        private static TMP_SpriteCharacter CreateCharacter(uint unicode, string name, float scale, TMP_SpriteGlyph glyph)
        {
            return new TMP_SpriteCharacter(unicode, glyph)
            {
                name = name,
                scale = scale
            };
        }

        private static GlyphRect GetRect(Sprite sprite)
        {
            Rect rect = sprite.rect;
            return new GlyphRect(Mathf.RoundToInt(rect.x), Mathf.RoundToInt(rect.y), Mathf.RoundToInt(rect.width), Mathf.RoundToInt(rect.height));
        }

        // New sprites copy the metrics of an existing glyph of the same size, so hand-tuned bearings carry over.
        private static GlyphMetrics GetMetrics(Sprite sprite, List<TMP_SpriteGlyph> oldGlyphs)
        {
            GlyphRect rect = GetRect(sprite);
            TMP_SpriteGlyph match = oldGlyphs.FirstOrDefault(g => g.glyphRect.width == rect.width && g.glyphRect.height == rect.height);
            if (match != null)
            {
                return match.metrics;
            }

            return new GlyphMetrics(rect.width, rect.height, -sprite.pivot.x, rect.height - sprite.pivot.y, rect.width);
        }
    }
}
