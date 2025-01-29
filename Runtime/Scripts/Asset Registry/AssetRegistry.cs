using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
using Object = UnityEngine.Object;

namespace HHG.Common.Runtime
{
    public class AssetRegistry : SingletonScriptableObject<AssetRegistry>, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct AssetFolder
        {
            public string Folder;
            public string Filter;
        }

        public static bool LoadBeforeBuild => Instance.loadBeforeBuild;

        [SerializeField] private bool loadBeforeBuild;
        [SerializeField] private AssetFolder[] folders;
        [SerializeField] private SerializableDictionary<string, Object> assets = new SerializableDictionary<string, Object>();

        // This doesn't need to be serialzied since it gets initialized after deserialize
        private Dictionary<Object, string> guids = new Dictionary<Object, string>();

        public static T GetAsset<T>(string guid) where T : Object => Instance.getAsset<T>(guid);
        public static string GetGuid(Object asset) => Instance.getGuid(asset);

        public T getAsset<T>(string guid) where T : Object
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            if (assets.TryGetValue(guid, out var asset))
            {
                return asset as T;
            }

#if UNITY_EDITOR
            editorLoad();
            assets.TryGetValue(guid, out asset);
#endif

            return asset as T;
        }


        public string getGuid(Object asset)
        {
            if (asset == null)
            {
                return null;
            }

            if (guids.TryGetValue(asset, out var guid))
            {
                return guid;
            }

#if UNITY_EDITOR
            editorLoad();
            guids.TryGetValue(asset, out guid);
#endif

            return guid;
        }


        public void OnBeforeSerialize()
        {
            // Do nothing
        }

        public void OnAfterDeserialize()
        {
            guids = assets.ToDictionary(kv => kv.Value, kv => kv.Key);
        }

#if UNITY_EDITOR

        public static void EditorLoad() => Instance.editorLoad();

        [ContextMenu("Load Assets")]
        private void editorLoad()
        {
            assets.Clear();
            guids.Clear();
            foreach (AssetFolder folder in folders)
            {
                string filter = string.Join(' ', folder.Filter.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(f => $"t:{f}"));
                string[] ids = AssetDatabase.FindAssets($"{filter}", new[] { folder.Folder });
                var paths = ids.Select(id => AssetDatabase.GUIDToAssetPath(id));
                var objs = paths.Select(path => AssetDatabase.LoadAssetAtPath<Object>(path));
                foreach (Object obj in objs)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    string id = AssetDatabase.AssetPathToGUID(path);

                    if (assets.ContainsKey(id))
                    {
                        Debug.LogError("Asset registry already contains asset: " + path, obj);
                        continue;
                    }

                    assets.Add(id, obj);
                    guids.Add(obj, id);
                }
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Half Human Games/Asset Registry")]
        private static void Open()
        {
            Selection.activeObject = Instance;
        }

#endif

    }
}