using HHG.Common.Runtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownDrawer : PropertyDrawer
    {
        private DropdownAttribute filter => attribute as DropdownAttribute;
        private List<Object> choiceAssets = new List<Object>();
        private List<string> choiceNames = new List<string>();

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                PropertyField propertyField = new PropertyField(property);
                container.Add(propertyField);
            }
            else
            {
                RefreshDropdownValues(property);
                DropdownField dropdownField = new DropdownField(property.displayName, choiceNames, 0);
                dropdownField.AddToClassList(BaseField<string>.alignedFieldUssClassName);
                dropdownField.AddManipulator(CreateDropdownContextMenu(dropdownField));
                dropdownField.UnregisterValueChangedCallback(OnSpawnDropdownChanged);
                dropdownField.RegisterValueChangedCallback(OnSpawnDropdownChanged);
                dropdownField.SetValueWithoutNotify(property.objectReferenceValue?.name ?? "None");
                dropdownField.userData = property;
                container.Add(dropdownField);
            }

            return container;
        }

        private void RefreshDropdownValues(SerializedProperty property)
        {
            System.Type type = filter.Type ?? property.GetPropertyType();
            DropdownUtil.GetChoiceList(ref choiceAssets, ref choiceNames, t => t.IsBaseImplementationOf(type), filter.filter);
        }

        private ContextualMenuManipulator CreateDropdownContextMenu(VisualElement visualElement) => new ContextualMenuManipulator((evt) =>
        {
            evt.menu.AppendAction("Refresh", (x) => RefreshDropdownValues((SerializedProperty)visualElement.userData), DropdownMenuAction.AlwaysEnabled, visualElement);
        });

        private void OnSpawnDropdownChanged(ChangeEvent<string> evt)
        {
            DropdownField dropdownField = evt.currentTarget as DropdownField;
            SerializedProperty property = dropdownField.userData as SerializedProperty;
            int i = choiceNames.IndexOf(evt.newValue);

            // For some reason this event triggers for the label change as well
            if (i != -1)
            {
                property.objectReferenceValue = choiceAssets[i];
                property.serializedObject.ApplyModifiedProperties();
                dropdownField.SetValueWithoutNotify(property.objectReferenceValue?.name ?? "None");
            }
        }
    }
}
