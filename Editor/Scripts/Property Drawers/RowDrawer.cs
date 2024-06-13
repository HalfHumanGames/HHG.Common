using HHG.Common.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(RowAttribute))]
    public class RowDrawer : PropertyDrawer
    {
        // TODO: Make auto do child if child count is 1 or full otherwise
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;

            SerializedProperty child = property.Copy();
            SerializedProperty end = property.GetEndProperty();
            child.NextVisible(true);

            bool first = true;
            do
            {
                // Neither the constructor nor the label field work
                // There seems to be no means to set the label text
                PropertyField field = new PropertyField(child, property.displayName);

                // Not sure how to get all elements the same width
                field.style.flexGrow = 1f;

                // Use GeometryChangedEvent workaround to set the label
                field.userData = first ? property.displayName : string.Empty;
                field.UnregisterCallback<GeometryChangedEvent>(OnGeometryChangeEvent);
                field.RegisterCallback<GeometryChangedEvent>(OnGeometryChangeEvent);

                container.Add(field);

                first = false;
            } while (child.NextVisible(false) && !SerializedProperty.EqualContents(child, end));

            return container;
        }

        // No idea why, but GeometryChangedEvent works, but AttachToPanelEvent doesn't
        private static void OnGeometryChangeEvent(GeometryChangedEvent evt)
        {
            PropertyField field = evt.target as PropertyField;

            if (field.Q<Label>() is Label label)
            {
                label.text = field.userData as string;
            }
        }
    }
}
