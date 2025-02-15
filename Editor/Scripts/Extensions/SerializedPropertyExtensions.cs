using HHG.Common.Runtime;
using System;
using System.Text.RegularExpressions;
using UnityEditor;

namespace HHG.Common.Editor
{
    public static class SerializedPropertyExtensions
    {
        public static SerializedProperty GetParentProperty(this SerializedProperty property)
        {
            int index = property.propertyPath.LastIndexOf('.');
            string path = property.propertyPath[..index];
            return property.serializedObject.FindProperty(path);
        }

        public static Type GetPropertyType(this SerializedProperty property)
        {
            string pattern = @"PPtr<\$?(.*?)>";
            string typeName = Regex.Replace(property.type, pattern, "$1");
            Type type = TypeUtil.FindType(typeName);
            return type;
        }

        public static int CountVisibleInProperty(this SerializedProperty property)
        {
            int count = 1;
            SerializedProperty childProperty = property.Copy();
            SerializedProperty endProperty = property.GetEndProperty();

            if (childProperty.NextVisible(true))
            {
                do
                {
                    if (childProperty.propertyType == SerializedPropertyType.Generic && !childProperty.hasVisibleChildren)
                    {
                        continue;
                    }

                    count++;

                } while (childProperty.NextVisible(false) && !SerializedProperty.EqualContents(childProperty, endProperty));
            }

            return count;
        }
    }
}