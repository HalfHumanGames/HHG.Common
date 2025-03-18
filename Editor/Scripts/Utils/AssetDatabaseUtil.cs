using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
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

        public static string GetProjectFolderAbsolutePath()
        {
            return ToAbsolutePath(GetProjectFolderAssetPath());
        }

        public static string GetProjectFolderAssetPath()
        {
            string path = "Assets";

            Object activeObject = Selection.activeObject;

            if (activeObject != null)
            {
                path = AssetDatabase.GetAssetPath(activeObject);

                if (!string.IsNullOrEmpty(path) && !AssetDatabase.IsValidFolder(path))
                {
                    path = Path.GetDirectoryName(path);
                }
            }

            return path;
        }

        public static string GetAbsolutePath(Object asset)
        {
            return ToAbsolutePath(AssetDatabase.GetAssetPath(asset));
        }

        public static string ToAbsolutePath(string assetPath)
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            return Path.Combine(projectPath, assetPath).Replace('/', Path.DirectorySeparatorChar);
        }
    }
}