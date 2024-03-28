using HHG.Common.Runtime;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(FilterAttribute))]
    public class FilterDrawer : PropertyDrawer
    {
        private FilterAttribute filter => attribute as FilterAttribute;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();

            if (property.propertyType != SerializedPropertyType.ObjectReference || filter.Type == null)
            {
                PropertyField propertyField = new PropertyField(property);
                container.Add(propertyField);
            }
            else
            {
                System.Type[] types = typeof(Object).FindSubclasses(t => t.Implements(filter.Type)).ToArray();

                if (types.Length == 0)
                {
                    Label label = new Label($"No UnityEngine.Object subclass implements '{filter.Type}'");
                    container.Add(label);
                }
                else
                {
                    Object value = property.objectReferenceValue;
                    System.Type type = value && types.Contains(value.GetType()) ? value.GetType() : types[0];

                    ObjectField objectField = new ObjectField(property.displayName);
                    objectField.objectType = type;
                    objectField.SetValueWithoutNotify(value);

                    if (types.Length > 1)
                    {
                        objectField.AddManipulator(CreateDropdownContextMenu(objectField, types));
                    }

                    objectField.RegisterValueChangedCallback(evt =>
                    {
                        property.objectReferenceValue = evt.newValue;
                        property.serializedObject.ApplyModifiedProperties();
                    });

                    container.Add(objectField);
                }
            }

            return container;
        }

        private ContextualMenuManipulator CreateDropdownContextMenu(ObjectField objectField, System.Type[] types) => new ContextualMenuManipulator((evt) =>
        {
            foreach (System.Type type in types)
            {
                evt.menu.AppendAction(ObjectNames.NicifyVariableName(type.Name), (x) =>
                {
                    (ObjectField field, System.Type t) = ((ObjectField, System.Type))x.userData;

                    if (field.objectType != t)
                    {
                        field.objectType = t;
                        field.SetValueWithoutNotify(null);
                    }
                }, DropdownMenuAction.AlwaysEnabled, (objectField, type));
            }
        });
    }
}