using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static string resourcePath => typeof(T).Name.ToNicified();
        private static string assetPath => $"Assets/Resources/{resourcePath}.asset";

        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null || instance.Equals(null))
                {
                    // Resources.Load does NOT work with string.Empty
                    instance = Resources.Load<T>(resourcePath);
                    
                    if (instance == null)
                    {
                        // Resources.LoadAll DOES work with string.Empty
                        // This searches all Resources folders in the project
                        instance = Resources.LoadAll<T>(string.Empty).FirstOrDefault();
                    }

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
                        Debug.LogWarning($"{typeof(T)} not found, so a new one has been created at: {assetPath}");
                        instance = CreateInstance<T>();
#if UNITY_EDITOR
                        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                        {
                            AssetDatabase.CreateFolder("Assets", "Resources");
                        }
                        AssetDatabase.CreateAsset(instance, assetPath);
#endif
                    }
                }
                return instance;
            }
        }
    }
}
