using System;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace HHG.Common.Editor
{
    public static class AssetDatabaseUtil
    {
        public static string GetGuid(Object obj)
        {
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
        }

        public static T FindAsset<T>() where T : Object
        {
            Type type = typeof(T);
            string guid = AssetDatabase.FindAssets($"t:{type.Name}").FirstOrDefault();
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static T[] FindAssets<T>() where T : Object
        {
            Type type = typeof(T);
            string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");
            string[] paths = guids.Select(g => AssetDatabase.GUIDToAssetPath(g)).ToArray();
            return paths.Select(p => AssetDatabase.LoadAssetAtPath<T>(p)).ToArray();
        }
    }
}