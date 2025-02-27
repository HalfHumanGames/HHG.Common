using HHG.Common.Runtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(DropdownAttribute), true)]
    public class DropdownDrawer : PropertyDrawer
    {
        private static Dictionary<System.Type, Object[]> assetCache = new Dictionary<System.Type, Object[]>();
        private static Dictionary<System.Type, string[]> nameCache = new Dictionary<System.Type, string[]>();

        private DropdownAttribute filter => attribute as DropdownAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            System.Type type = filter.Type ?? property.GetPropertyType();

            Object[] assets;
            string[] names;

            if (!assetCache.TryGetValue(type, out assets) || !nameCache.TryGetValue(type, out names))
            {
                assets = new Object[0];
                names = new string[0];

                DropdownUtil.GetChoiceArray(ref assets, ref names, t => t.IsBaseImplementationOf(type), filter.Filter);

                // Cache for subsequent lookups
                assetCache[type] = assets;
                nameCache[type] = names;
            }

            int currentIndex = Mathf.Max(0, System.Array.IndexOf(assets, property.objectReferenceValue));

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
                    RefreshDropdownValues(property, ref assets, ref names, ref currentIndex);
                }

                EditorGUI.EndProperty();
            }
        }

        private void RefreshDropdownValues(SerializedProperty property, ref Object[] assets, ref string[] names, ref int currentIndex)
        {
            System.Type type = filter.Type ?? property.GetPropertyType();

            DropdownUtil.GetChoiceArray(ref assets, ref names, t => t.IsBaseImplementationOf(type), filter.Filter);

            // Cache for subsequent lookups
            assetCache[type] = assets;
            nameCache[type] = names;

            currentIndex = Mathf.Max(0, System.Array.IndexOf(assets, property.objectReferenceValue));
        }
    }
}
