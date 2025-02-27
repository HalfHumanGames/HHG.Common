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
            totalHeight = 0f;

            property.isExpanded = true;
            position.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty childProperty = property.Copy();
            SerializedProperty endProperty = property.GetEndProperty();

            if (childProperty.NextVisible(true) && !SerializedProperty.EqualContents(childProperty, endProperty))
            {
                UnfoldName unfoldName = unfold.Name;

                if (unfoldName == UnfoldName.Auto)
                {
                    bool hasSingleChild = property.Copy().CountVisibleInProperty() - 1 == 1;
                    unfoldName = hasSingleChild ? UnfoldName.Parent : UnfoldName.Child;
                }

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
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return totalHeight - EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
