using HHG.Common.Runtime;
using System;
using System.Text.RegularExpressions;
using UnityEditor;

namespace HHG.Common.Editor
{
    public static class SerializedPropertyExtensions
    {
        public static Type GetPropertyType(this SerializedProperty property)
        {
            string pattern = @"PPtr<\$?(.*?)>";
            string typeName = Regex.Replace(property.type, pattern, "$1");
            Type type = TypeUtil.FindType(typeName);
            return type;
        }
    }
}