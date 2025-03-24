using UnityEngine;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HHG.Common.Runtime
{
    public class AssetRegistry : ScriptableSingleton<AssetRegistry>, ISerializationCallbackReceiver
    {
        [System.Serializable]
        public struct AssetFolder
        {
            [HideInInspector] public string _; // So has default label
            public string Folder;
            public string Filter;
        }

        public static bool LoadBeforeBuild => Instance.loadBeforeBuild;

        [SerializeField] private bool loadBeforeBuild = true;
        [SerializeField] private AssetFolder[] folders;
        [SerializeField] private SerializedDictionary<string, Object> assets = new SerializedDictionary<string, Object>();

        // This doesn't need to be serialzied since it gets initialized after deserialize
        private Dictionary<Object, string> guids = new Dictionary<Object, string>();

        public static T GetAsset<T>(string guid) where T : Object => Instance.getAsset<T>(guid);
        public static string GetGuid(Object asset) => Instance.getGuid(asset);

        public T getAsset<T>(string guid) where T : Object
        {
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"Guid cannot be null or empty.");
                return null;
            }

            if (assets.TryGetValue(guid, out var asset))
            {
                return asset as T;
            }

#if UNITY_EDITOR
            editorLoad();

            if (assets.TryGetValue(guid, out asset))
            {
                return asset as T;
            }
#endif

            Debug.LogError($"Asset not found for guid: {guid}");

            return null;
        }


        public string getGuid(Object asset)
        {
            if (asset == null)
            {
                Debug.LogError($"Asset cannot be null.");
                return null;
            }

            if (guids.TryGetValue(asset, out var guid))
            {
                return guid;
            }

#if UNITY_EDITOR
            editorLoad();

            if (guids.TryGetValue(asset, out guid))
            {
                return guid;
            }
#endif
            
            Debug.LogError($"Guid not found for asset: {asset.name}");

            return string.Empty;            
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
                string filter = string.Join(' ', folder.Filter.Split(",", System.StringSplitOptions.RemoveEmptyEntries).Select(f => $"t:{f}"));
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

        [MenuItem("| Half Human Games |/Asset Registry")]
        private static void Open()
        {
            Selection.activeObject = Instance;
        }

#endif

    }
}