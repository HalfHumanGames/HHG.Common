using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class SubAssetList<T> where T : ScriptableObject
    {
        public IReadOnlyList<T> Items => items;

        [SerializeField] private List<string> names = new List<string>();
        [SerializeField] private List<T> items = new List<T>();

        public void SyncSubAssetList(Object parent)
        {
            items.Clear();
            Object[] children = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(parent));

            foreach (Object asset in children)
            {
                if (asset is T child)
                {
                    items.Add(child);
                }
            }

            for (int i = 0; i < names.Count; i++)
            {
                if (i < items.Count)
                {
                    if (items[i].name != names[i])
                    {
                        items[i].name = names[i];
                        EditorUtility.SetDirty(items[i]);
                    }
                }
                else
                {
                    T child = ScriptableObject.CreateInstance<T>();
                    child.name = names[i];
                    items.Add(child);
                    AssetDatabase.AddObjectToAsset(child, parent);
                    EditorUtility.SetDirty(child);
                }
            }

            for (int i = items.Count - 1; i >= names.Count; i--)
            {
                Object.DestroyImmediate(items[i], true);
                items.RemoveAt(i);
            }

            AssetDatabase.SaveAssets();
        }
    }
}