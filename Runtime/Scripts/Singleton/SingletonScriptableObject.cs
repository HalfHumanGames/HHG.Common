#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        private static string resourcePath => $"{typeof(T)}";
        private static string defaultAssetPath => $"Assets/Resources/{resourcePath}.asset";

        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null || instance.Equals(null))
                {
                    instance = Resources.Load<T>(string.Empty);
#if UNITY_EDITOR
                    if (instance == null)
                    {
                        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
                        if (guids.Length > 0)
                        {
                            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                            instance = AssetDatabase.LoadAssetAtPath<T>(path);
                        }
                    }
#endif
                    if (instance == null)
                    {
                        Debug.LogWarning($"{typeof(T)} not found, so a new one has been created at: {defaultAssetPath}");
                        instance = CreateInstance<T>();
#if UNITY_EDITOR
                        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                        {
                            AssetDatabase.CreateFolder("Assets", "Resources");
                        }
                        AssetDatabase.CreateAsset(instance, defaultAssetPath);
#endif
                    }
                }
                return instance;
            }
        }
    }
}
