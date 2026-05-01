using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    public class EditorScriptableSingleton<T> : ScriptableObject where T : EditorScriptableSingleton<T>
    {
        private static string assetName => typeof(T).FullName;
        private static string assetPath => $"Assets/Editor Resources/{assetName}.asset";

        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null || instance.Equals(null))
                {
                    instance = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                    if (instance == null)
                    {
                        Debug.LogWarning($"{typeof(T)} not found, so a new one has been created at: {assetPath}");
                        instance = CreateInstance<T>();

                        if (!AssetDatabase.IsValidFolder("Assets/Editor Resources"))
                        {
                            AssetDatabase.CreateFolder("Assets", "Editor Resources");
                        }
                        AssetDatabase.CreateAsset(instance, assetPath);
                    }
                }
                return instance;
            }
        }
    }
}