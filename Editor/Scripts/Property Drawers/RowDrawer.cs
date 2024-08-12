using HHG.Common.Runtime;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(RowAttribute), true)]
    public class RowDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);

            // Force expand so children are visible
            property.isExpanded = true; 

            SerializedProperty childProperty = property.Copy();
            SerializedProperty endProperty = property.GetEndProperty();
            childProperty.NextVisible(true);

            int childCount = property.Copy().CountInProperty() - 1;
            float width = position.width / childCount;
            Rect rect = new Rect(position.x, position.y, width, position.height);

            do
            {
                EditorGUI.PropertyField(rect, childProperty, GUIContent.none);
                rect.x += width;

            } while (childProperty.NextVisible(false) && !SerializedProperty.EqualContents(childProperty, endProperty));

            EditorGUI.EndProperty();
        }
    }
}
