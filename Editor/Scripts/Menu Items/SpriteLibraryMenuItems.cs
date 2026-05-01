using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace HHG.Common.Editor
{
    public static class SpriteLibraryMenuItems
    {
        [MenuItem("CONTEXT/SpriteLibrary/Sync Categories")]
        private static void SyncCategoriesMenuItem(MenuCommand command)
        {
            if (command.context is SpriteLibrary comp) SyncCategories(comp);
        }

        [MenuItem("CONTEXT/SpriteLibrary/Sync Labels")]
        private static void SyncLabelsMenuItem(MenuCommand command)
        {
            if (command.context is SpriteLibrary comp) SyncLabels(comp);
        }

        [MenuItem("CONTEXT/SpriteLibrary/Sync Resolvers")]
        private static void SyncResolversMenuItem(MenuCommand command)
        {
            if (command.context is SpriteLibrary comp) SyncResolvers(comp);
        }

        [MenuItem("CONTEXT/SpriteLibrary/Sync All")]
        private static void SyncAllMenuItem(MenuCommand command)
        {
            if (command.context is SpriteLibrary comp) SyncAll(comp);
        }

        private static void SyncCategories(SpriteLibrary comp)
        {
            SpriteLibraryAsset library = comp.spriteLibraryAsset;
            if (library == null)
            {
                Debug.LogError($"SpriteLibrary on '{comp.gameObject.name}' has no SpriteLibraryAsset assigned.", comp);
                return;
            }

            SpriteRenderer[] renderers = comp.GetComponentsInChildren<SpriteRenderer>(true);
            if (renderers.Length == 0)
            {
                Debug.LogWarning($"No SpriteRenderer children found on '{comp.gameObject.name}'.", comp);
                return;
            }

            HashSet<string> expected = new HashSet<string>(renderers.Select(sr => sr.gameObject.name));
            List<string> existing = library.GetCategoryNames().ToList();
            int removed = 0, added = 0;

            foreach (string category in existing.Where(c => !expected.Contains(c)))
            {
                List<string> labels = library.GetCategoryLabelNames(category).ToList();

                foreach (string label in labels)
                {
                    library.RemoveCategoryLabel(category, label, deleteCategory: label == labels.Last());
                }

                removed++;
            }

            foreach (string category in expected.Where(c => !existing.Contains(c)))
            {
                library.AddCategoryLabel(null, category, "None");
                added++;
            }

            SortCategoriesAlphabetically(library);
            Save(library);

            Debug.Log($"[SpriteLibrary] Sync Categories '{library.name}': +{added} -{removed}", comp);
        }

        private static void SyncLabels(SpriteLibrary comp)
        {
            SpriteLibraryAsset library = comp.spriteLibraryAsset;
            if (library == null)
            {
                Debug.LogError($"SpriteLibrary on '{comp.gameObject.name}' has no SpriteLibraryAsset assigned.", comp);
                return;
            }

            // Sort by descending length so longer category names match before shorter ones
            List<string> categories = library.GetCategoryNames().OrderByDescending(c => c.Length).ToList();
            if (categories.Count == 0)
            {
                Debug.LogWarning($"No categories in '{library.name}'. Run 'Sync Categories' first.", comp);
                return;
            }

            string[] guids = AssetDatabase.FindAssets("t:Sprite");
            if (guids.Length == 0)
            {
                Debug.LogWarning($"No sprites found.", comp);
            }

            // Map category to label to sprite using longest-match prefix
            Dictionary<string, Dictionary<string, Sprite>> found = categories.ToDictionary(c => c, _ => new Dictionary<string, Sprite>());

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

                if (sprite == null) continue;

                foreach (string category in categories)
                {
                    string prefix = category + " - ";
                    if (sprite.name.StartsWith(prefix))
                    {
                        string label = sprite.name.Substring(prefix.Length);
                        if (!string.IsNullOrWhiteSpace(label))
                        {
                            found[category][label] = sprite;
                        }
                        break;
                    }
                }
            }

            int removed = 0, added = 0, updated = 0;

            foreach (string category in categories)
            {
                Dictionary<string, Sprite> foundLabels = found[category];

                foreach (string label in library.GetCategoryLabelNames(category).ToList())
                {
                    if (label != "None" && !foundLabels.ContainsKey(label))
                    {
                        library.RemoveCategoryLabel(category, label, deleteCategory: false);
                        removed++;
                    }
                }

                foreach (KeyValuePair<string, Sprite> kvpair in foundLabels)
                {
                    Sprite existing = library.GetSprite(category, kvpair.Key);

                    if (existing == kvpair.Value) continue;

                    library.AddCategoryLabel(kvpair.Value, category, kvpair.Key);

                    if (existing == null) added++; else updated++;
                }
            }

            Save(library);

            Debug.Log($"[SpriteLibrary] Sync Labels '{library.name}': +{added} ~{updated} -{removed}", comp);
        }

        private static void SyncAll(SpriteLibrary comp)
        {
            SyncCategories(comp);
            SyncLabels(comp);
            SyncResolvers(comp);
        }

        private static void SyncResolvers(SpriteLibrary comp)
        {
            SpriteRenderer[] renderers = comp.GetComponentsInChildren<SpriteRenderer>(true);
            if (renderers.Length == 0)
            {
                Debug.LogWarning($"No SpriteRenderer children found on '{comp.gameObject.name}'.", comp);
                return;
            }

            int added = 0, updated = 0;
            foreach (SpriteRenderer sr in renderers)
            {
                SpriteResolver resolver = sr.GetComponent<SpriteResolver>();
                string expected = sr.gameObject.name;

                if (resolver == null)
                {
                    resolver = Undo.AddComponent<SpriteResolver>(sr.gameObject);
                    resolver.SetCategoryAndLabel(expected, "None");
                    EditorUtility.SetDirty(sr.gameObject);
                    added++;
                }
                else if (resolver.GetCategory() != expected)
                {
                    Undo.RecordObject(resolver, "Sync Resolver Category");
                    resolver.SetCategoryAndLabel(expected, resolver.GetLabel());
                    EditorUtility.SetDirty(resolver);
                    updated++;
                }
            }

            if (added > 0 || updated > 0) AssetDatabase.SaveAssets();

            Debug.Log($"[SpriteLibrary] Sync Resolvers '{comp.gameObject.name}': +{added} ~{updated}", comp);
        }

        private static void SortCategoriesAlphabetically(SpriteLibraryAsset library)
        {
            FieldInfo field = typeof(SpriteLibraryAsset).GetField("m_Labels", BindingFlags.Instance | BindingFlags.NonPublic);
            System.Collections.IList list = (System.Collections.IList)field?.GetValue(library);

            if (list == null || list.Count < 2) return;

            PropertyInfo nameProp = list[0].GetType().GetProperty("name");

            List<object> items = new List<object>();
            foreach (object item in list)
            {
                items.Add(item);
            }

            items.Sort((a, b) => string.Compare((string)nameProp.GetValue(a), (string)nameProp.GetValue(b), System.StringComparison.Ordinal));

            for (int i = 0; i < items.Count; i++)
            {
                list[i] = items[i];
            }
        }

        private static void Save(SpriteLibraryAsset library)
        {
            string path = AssetDatabase.GetAssetPath(library);
            library.SaveAsSourceAsset(path);
            AssetDatabase.ImportAsset(path);
        }
    }
}
