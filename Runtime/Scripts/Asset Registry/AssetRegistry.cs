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

        [SerializeField] private AssetFolder[] folders;
        [SerializeField] private SerializableDictionary<string, Object> assets = new SerializableDictionary<string, Object>();
        
        private Dictionary<Object, string> guids = new Dictionary<Object, string>();

        public static T GetAsset<T>(string guid) where T : Object => Instance.getAsset<T>(guid);
        public static string GetGuid(Object asset) => Instance.getGuid(asset);

        public T getAsset<T>(string guid) where T : Object => !string.IsNullOrEmpty(guid) && assets.ContainsKey(guid) ? assets[guid] as T : null;
        public string getGuid(Object asset) => asset != null && guids.ContainsKey(asset) ? guids[asset] : null;

        public void OnBeforeSerialize()
        {
            // Do nothing
        }

        public void OnAfterDeserialize()
        {
            guids = assets.ToDictionary(kv => kv.Value, kv => kv.Key);
        }

#if UNITY_EDITOR

        [ContextMenu("Load Assets")]
        private void LoadAssets()
        {
            assets.Clear();
            foreach (AssetFolder folder in folders)
            {
                string[] guids = AssetDatabase.FindAssets($"t:{folder.Filter}", new[] { folder.Folder });
                var objs = guids.Select(g => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(g)));
                foreach (Object obj in objs)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    if (assets.ContainsKey(guid))
                    {
                        Debug.LogError("Asset registry already contains asset: " + path, obj);
                        continue;
                    }
                    assets.Add(guid, obj);
                }
            }
        }

        [MenuItem("Half Human Games/Asset Registry")]
        private static void Open()
        {
            Selection.activeObject = Instance;
        }

#endif

    }
}