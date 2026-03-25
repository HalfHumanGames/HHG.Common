using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    public static class EditorUtil
    {
        public static string GetActiveFolderPath()
        {
            Type type = typeof(ProjectWindowUtil);
            MethodInfo getter = type.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            return getter.Invoke(null, new object[0]).ToString();
        }

        public static bool CanValidate()
        {
            return !Application.isPlaying && !EditorApplication.isCompiling && !EditorApplication.isUpdating && !BuildPipeline.isBuildingPlayer;
        }
    }
}