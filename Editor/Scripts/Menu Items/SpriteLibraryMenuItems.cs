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
        private const string menuRoot = "| Half Human Games |/Tools/Sprite Library/";

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

        [MenuItem(menuRoot + "Sync Labels")]
        private static void SyncLabelsMenuItem2()
        {
            switch (Selection.activeObject)
            {
                case SpriteLibrary comp:
                    SyncLabels(comp);
                    break;
                case SpriteLibraryAsset asset:
                    SyncLabels(asset);
                    break;
            }
        }

        [MenuItem(menuRoot + "Sync Labels", validate = true)]
        private static bool SyncLabelsValidate()
        {
            return Selection.activeObject is SpriteLibrary ||
                   Selection.activeObject is SpriteLibraryAsset;
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

        private static void SyncLabels(SpriteLibrary library) => SyncLabels(library.spriteLibraryAsset, library);
        private static void SyncLabels(SpriteLibraryAsset library) => SyncLabels(library, library);
        private static void SyncLabels(SpriteLibraryAsset library, Object context)
        {
            if (library == null)
            {
                Debug.LogError($"SpriteLibrary on '{context.name}' has no SpriteLibraryAsset assigned.", context);
                return;
            }

            // Sort by descending length so longer category names match before shorter ones
            List<string> categories = library.GetCategoryNames().OrderByDescending(c => c.Length).ToList();
            if (categories.Count == 0)
            {
                Debug.LogWarning($"No categories in '{library.name}'. Run 'Sync Categories' first.", context);
                return;
            }

            string libraryPath = AssetDatabase.GetAssetPath(library);
            string[] pathParts = libraryPath.Split('/');
            string searchFolder = pathParts.Length > 1 ? $"{pathParts[0]}/{pathParts[1]}" : "Assets";
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { searchFolder });
            if (guids.Length == 0)
            {
                Debug.LogWarning($"No sprites found in '{searchFolder}'.", context);
            }

            // Map category to label to sprite using longest-match prefix
            Dictionary<string, Dictionary<string, Sprite>> found = categories.ToDictionary(c => c, _ => new Dictionary<string, Sprite>());

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>();

                foreach (Sprite sprite in sprites)
                {
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

            Debug.Log($"[SpriteLibrary] Sync Labels '{library.name}': +{added} ~{updated} -{removed}", context);
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

            SpriteLibraryAsset library = comp.spriteLibraryAsset;

            int added = 0, updated = 0;
            foreach (SpriteRenderer sr in renderers)
            {
                SpriteResolver resolver = sr.GetComponent<SpriteResolver>();
                string expected = sr.gameObject.name;
                string currentCategory = resolver.GetCategory();
                string currentLabel = resolver.GetLabel();

                if (resolver == null)
                {
                    string defaultLabel = GetDefaultLabel(library, expected);
                    resolver = Undo.AddComponent<SpriteResolver>(sr.gameObject);
                    resolver.SetCategoryAndLabel(expected, defaultLabel);
                    EditorUtility.SetDirty(sr.gameObject);
                    added++;
                }
                else if (currentCategory != expected)
                {
                    string defaultLabel = GetDefaultLabel(library, expected);
                    string newLabel = (currentLabel != "None" && currentLabel != null) ? currentLabel : defaultLabel;
                    Undo.RecordObject(resolver, "Sync Resolver Category");
                    resolver.SetCategoryAndLabel(expected, newLabel);
                    EditorUtility.SetDirty(resolver);
                    updated++;
                }
                else if (currentLabel == "None")
                {
                    string defaultLabel = GetDefaultLabel(library, expected);
                    string newLabel = (currentLabel != "None" && currentLabel != null) ? currentLabel : defaultLabel;
                    if (newLabel != "None")
                    {
                        Undo.RecordObject(resolver, "Sync Resolver Category");
                        resolver.SetCategoryAndLabel(expected, newLabel);
                        EditorUtility.SetDirty(resolver);
                        updated++;
                    }
                }
            }

            if (added > 0 || updated > 0) AssetDatabase.SaveAssets();

            Debug.Log($"[SpriteLibrary] Sync Resolvers '{comp.gameObject.name}': +{added} ~{updated}", comp);
        }

        private static string GetDefaultLabel(SpriteLibraryAsset library, string category)
        {
            string first = library.GetCategoryLabelNames(category).FirstOrDefault(l => l != "None");
            return first ?? "None";
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
