using HHG.Common.Runtime;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(UnfoldAttribute), true)]
    public class UnfoldDrawer : PropertyDrawer
    {
        private UnfoldAttribute unfold => attribute as UnfoldAttribute;

        private float totalHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Force expand so children are visible
            property.isExpanded = true;
            position.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty childProperty = property.Copy();
            SerializedProperty endProperty = property.GetEndProperty();
            childProperty.NextVisible(true);

            UnfoldName unfoldName = unfold.Name;

            if (unfoldName == UnfoldName.Auto)
            {
                unfoldName = property.Copy().CountInProperty() - 1 == 1 ? UnfoldName.Parent : UnfoldName.Child;
            }

            totalHeight = 0f;
            do
            {
                string name = unfoldName switch
                {
                    UnfoldName.Child => childProperty.displayName,
                    UnfoldName.Parent => property.displayName,
                    UnfoldName.Full => $"{property.displayName} {childProperty.displayName}",
                    _ => string.Empty
                };

                EditorGUI.PropertyField(position, childProperty, new GUIContent(name), true);
                float height = EditorGUI.GetPropertyHeight(childProperty, true) + EditorGUIUtility.standardVerticalSpacing;
                position.y += height;
                totalHeight += height;

            } while (childProperty.NextVisible(false) && !SerializedProperty.EqualContents(childProperty, endProperty));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return totalHeight - EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
