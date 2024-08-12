using HHG.Common.Runtime;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(FilterAttribute), true)]
    public class FilterDrawer : PropertyDrawer
    {
        private FilterAttribute filter => attribute as FilterAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, filter.Type, true);

            EditorGUI.EndProperty();
        }
    }
}
