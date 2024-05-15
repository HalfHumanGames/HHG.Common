using HHG.Common.Runtime;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownDrawer : PropertyDrawer
    {
        private static Dictionary<Type, List<Object>> assetCache = new Dictionary<Type, List<Object>>();
        private static Dictionary<Type, List<string>> nameCache = new Dictionary<Type, List<string>>();

        private DropdownAttribute filter => attribute as DropdownAttribute;
        private List<Object> assets = new List<Object>();
        private List<string> names = new List<string>();

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
                DropdownField dropdownField = new DropdownField(property.displayName, names, 0);
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

        private void RefreshDropdownValues(SerializedProperty property, bool force = false)
        {
            Type type = filter.Type ?? property.GetPropertyType();
            if (!force && assetCache.ContainsKey(type) && nameCache.ContainsKey(type))
            {
                assets = assetCache[type];
                names = nameCache[type];     
            }
            else
            {
                DropdownUtil.GetChoiceList(ref assets, ref names, t => t.IsBaseImplementationOf(type), filter.filter);

                // Cache for subsequent lookups
                assetCache[type] = assets;
                nameCache[type] = names;
            }
        }

        private ContextualMenuManipulator CreateDropdownContextMenu(VisualElement visualElement) => new ContextualMenuManipulator((evt) =>
        {
            evt.menu.AppendAction("Refresh", (x) => RefreshDropdownValues((SerializedProperty)visualElement.userData, true), DropdownMenuAction.AlwaysEnabled, visualElement);
        });

        private void OnSpawnDropdownChanged(ChangeEvent<string> evt)
        {
            DropdownField dropdownField = evt.currentTarget as DropdownField;
            SerializedProperty property = dropdownField.userData as SerializedProperty;
            int i = names.IndexOf(evt.newValue);

            // For some reason this event triggers for the label change as well
            if (i != -1)
            {
                property.objectReferenceValue = assets[i];
                property.serializedObject.ApplyModifiedProperties();
                dropdownField.SetValueWithoutNotify(property.objectReferenceValue?.name ?? "None");
            }
        }
    }
}
