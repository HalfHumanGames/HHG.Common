using HHG.Common.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(UnfoldAttribute))]
    public class UnfoldDrawer : PropertyDrawer
    {
        private UnfoldAttribute unfold => attribute as UnfoldAttribute;

        // TODO: Make auto do child if child count is 1 or full otherwise
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();
            SerializedProperty childProperty = property.Copy();
            SerializedProperty endProperty = property.GetEndProperty();
            childProperty.NextVisible(true);
            do
            {
                string name = unfold.Name switch {
                    UnfoldName.Child => childProperty.displayName,
                    UnfoldName.Parent => property.displayName,
                    _ => $"{property.displayName} {childProperty.displayName}",
                };
                PropertyField field = new PropertyField(childProperty, name);
                field.Bind(childProperty.serializedObject);
                container.Add(field);
            } while (childProperty.NextVisible(false) && !SerializedProperty.EqualContents(childProperty, endProperty));
            return container;
        }
    }
}
