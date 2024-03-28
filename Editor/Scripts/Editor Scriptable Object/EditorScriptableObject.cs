using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    public class EditorScriptableObject<T> : ScriptableObject where T : EditorScriptableObject<T>
    {
        private static string assetPath => $"Assets/Editor Resources/{typeof(T)}.asset";

        protected static T LoadOrCreateInstance()
        {
            T instance = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (instance == null && !EditorApplication.isCompiling && !EditorApplication.isUpdating)
            {
                Debug.LogWarning($"{typeof(T)} asset not found at '{assetPath}' so a new one has been created.");
                instance = CreateInstance<T>();
                AssetDatabase.CreateAsset(instance, assetPath);
            }

            return instance;
        }
    }
}