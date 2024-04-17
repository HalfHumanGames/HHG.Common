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

        private SerializedProperty property;
        private DropdownField dropdownField;
        private List<ScriptableObject> choiceAssets = new List<ScriptableObject>();
        private List<string> choiceNames = new List<string>();

        public override VisualElement CreatePropertyGUI(SerializedProperty prop)
        {
            VisualElement container = new VisualElement();
            property = prop;

            if (property.propertyType != SerializedPropertyType.ObjectReference || filter.Type == null)
            {
                PropertyField propertyField = new PropertyField(property);
                container.Add(propertyField);
            }
            else
            {
                RefreshDropdownValues();
                dropdownField = new DropdownField(property.displayName, choiceNames, 0);
                dropdownField.AddToClassList(BaseField<string>.alignedFieldUssClassName);
                dropdownField.AddManipulator(CreateDropdownContextMenu(dropdownField));
                dropdownField.RegisterValueChangedCallback(OnSpawnDropdownChanged);
                dropdownField.SetValueWithoutNotify(property.objectReferenceValue?.name ?? "None");
                container.Add(dropdownField);
            }

            return container;
        }

        private void RefreshDropdownValues()
        {
            DropdownUtility.GetChoiceList(ref choiceAssets, ref choiceNames, t => t.IsBaseImplementationOf(filter.Type), filter.filter);
        }

        private ContextualMenuManipulator CreateDropdownContextMenu(VisualElement visualElement) => new ContextualMenuManipulator((evt) =>
        {
            evt.menu.AppendAction("Refresh", (x) => RefreshDropdownValues(), DropdownMenuAction.AlwaysEnabled, visualElement);
        });

        private void OnSpawnDropdownChanged(ChangeEvent<string> evt)
        {
            int i = choiceNames.IndexOf(evt.newValue);
            property.objectReferenceValue = choiceAssets[i];
            property.serializedObject.ApplyModifiedProperties();
            dropdownField.SetValueWithoutNotify(property.objectReferenceValue?.name ?? "None");
        }
    }
}
