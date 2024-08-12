using HHG.Common.Runtime;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(DropdownAttribute), true)]
    public class DropdownDrawer : PropertyDrawer
    {
        private static Dictionary<Type, List<Object>> assetCache = new Dictionary<Type, List<Object>>();
        private static Dictionary<Type, List<string>> nameCache = new Dictionary<Type, List<string>>();

        private DropdownAttribute filter => attribute as DropdownAttribute;
        private List<Object> assets = new List<Object>();
        private List<string> names = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property, label);
            }
            else
            {
                TryRefreshDropdownValues(property);

                int currentIndex = names.IndexOf(property.objectReferenceValue?.name ?? "None");

                if (currentIndex == -1) currentIndex = 0; // Default to "None"

                EditorGUI.BeginProperty(position, label, property);

                int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, names.ToArray());

                if (selectedIndex != currentIndex)
                {
                    property.objectReferenceValue = selectedIndex >= 0 && selectedIndex < assets.Count ? assets[selectedIndex] : null;
                    property.serializedObject.ApplyModifiedProperties();
                    TryRefreshDropdownValues(property, true);
                }

                EditorGUI.EndProperty();
            }
        }

        private void TryRefreshDropdownValues(SerializedProperty property, bool force = false)
        {
            Type type = filter.Type ?? property.GetPropertyType();

            if (!force && assetCache.ContainsKey(type) && nameCache.ContainsKey(type))
            {
                assets = assetCache[type];
                names = nameCache[type];
            }
            else
            {
                DropdownUtil.GetChoiceList(ref assets, ref names, t => t.IsBaseImplementationOf(type), filter.filter);

                // Cache for subsequent lookups
                assetCache[type] = assets;
                nameCache[type] = names;
            }

            // Add "None" option
            if (!names.Contains("None"))
            {
                names.Insert(0, "None");
                assets.Insert(0, null);
            }
        }
    }
}
