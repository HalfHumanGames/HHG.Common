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
        private static Dictionary<Type, Object[]> assetCache = new Dictionary<Type, Object[]>();
        private static Dictionary<Type, string[]> nameCache = new Dictionary<Type, string[]>();

        private DropdownAttribute filter => attribute as DropdownAttribute;
        private Object[] assets = new Object[0];
        private string[] names = new string[0];
        private int currentIndex;
        private bool initialized;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!initialized)
            {
                Initialize(property);
            }

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property, label);
            }
            else
            {
                EditorGUI.BeginProperty(position, label, property);

                int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, names);

                if (selectedIndex != currentIndex)
                {
                    currentIndex = selectedIndex;
                    property.objectReferenceValue = assets[currentIndex];
                    property.serializedObject.ApplyModifiedProperties();
                    RefreshDropdownValues(property);
                }

                EditorGUI.EndProperty();
            }
        }

        private void Initialize(SerializedProperty property)
        {
            initialized = true;

            Type type = filter.Type ?? property.GetPropertyType();

            if (!assetCache.TryGetValue(type, out assets) || !nameCache.TryGetValue(type, out names))
            {
                DropdownUtil.GetChoiceArray(ref assets, ref names, t => t.IsBaseImplementationOf(type), filter.Filter);

                // Cache for subsequent lookups
                assetCache[type] = assets;
                nameCache[type] = names;
            }

            currentIndex = Mathf.Max(0, Array.IndexOf(assets, property.objectReferenceValue));
        }

        private void RefreshDropdownValues(SerializedProperty property)
        {
            Type type = filter.Type ?? property.GetPropertyType();

            DropdownUtil.GetChoiceArray(ref assets, ref names, t => t.IsBaseImplementationOf(type), filter.Filter);

            // Cache for subsequent lookups
            assetCache[type] = assets;
            nameCache[type] = names;

            currentIndex = Mathf.Max(0, Array.IndexOf(assets, property.objectReferenceValue));
        }
    }
}
