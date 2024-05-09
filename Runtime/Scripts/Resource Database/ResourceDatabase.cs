using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HHG.Common.Runtime
{
    public class ResourceDatabase : SingletonScriptableObject<ResourceDatabase>
    {
        [SerializeField] private ResourceFolder[] resourceFolders;
        [SerializeField] private SerializableDictionary<string, Object> resources = new SerializableDictionary<string, Object>();

        public static T GetResourceByGuid<T>(string guid) where T : Object => Instance.getResourceByGuid<T>(guid);

        public T getResourceByGuid<T>(string guid) where T : Object
        {
            return resources.ContainsKey(guid) ? resources[guid] as T : null;
        }

#if UNITY_EDITOR

        [ContextMenu("Load Resources")]
        private void LoadResources()
        {
            resources.Clear();
            foreach (ResourceFolder resourceFolder in resourceFolders)
            {
                string[] guids = AssetDatabase.FindAssets($"t:{resourceFolder.TypeFilter}", new[] { resourceFolder.AssetFolder });
                var objs = guids.Select(g => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(g)));
                foreach (Object obj in objs)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    if (resources.ContainsKey(guid))
                    {
                        Debug.LogError("Resources already contains asset: " + path, obj);
                        continue;
                    }
                    resources.Add(guid, obj);
                }
            }
        }

        [MenuItem("Half Human Games/Resource Database")]
        private static void Open()
        {
            Selection.activeObject = Instance;
        }

#endif

    }
}