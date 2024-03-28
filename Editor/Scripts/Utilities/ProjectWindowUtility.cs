using System;
using System.Reflection;
using UnityEditor;

namespace HHG.Common.Editor
{
    public static class ProjectWindowUtility
    {
        public static string GetActiveFolderPath()
        {
            Type type = typeof(ProjectWindowUtil);
            MethodInfo getter = type.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            return getter.Invoke(null, new object[0]).ToString();
        }
    }
}