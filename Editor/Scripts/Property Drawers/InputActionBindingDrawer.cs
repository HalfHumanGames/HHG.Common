using HHG.Common.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(InputActionBinding), true)]
    public class InputActionBindingDrawer : PropertyDrawer
    {
        private Dictionary<InputActionReference, GUIContent[]> bindingOptions = new Dictionary<InputActionReference, GUIContent[]>();
        private Dictionary<InputActionReference, string[]> bindingOptionValues = new Dictionary<InputActionReference, string[]>();
        private int rows;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            rows = 1;

            SerializedProperty actionProperty = property.FindPropertyRelative("Action");
            SerializedProperty bindingIdProperty = property.FindPropertyRelative("BindingId");

            position = EditorGUI.PrefixLabel(position, label);

            Rect actionRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect bindingRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(actionRect, actionProperty, GUIContent.none);

            InputActionReference actionReference = (InputActionReference)actionProperty.objectReferenceValue;
            InputAction action = actionReference != null ? actionReference.action : null;

            if (action != null)
            {
                rows = 2;

                string bindingId = bindingIdProperty.stringValue;

                if (!bindingOptions.ContainsKey(actionReference) || !bindingOptionValues.ContainsKey(actionReference))
                {
                    RefreshBindingOptions(actionReference, bindingId);
                }

                int selectedBindingOption = bindingOptionValues[actionReference].IndexOf(bindingId);
                int newSelectedBinding = EditorGUI.Popup(bindingRect, selectedBindingOption, bindingOptions[actionReference]);
                if (newSelectedBinding != selectedBindingOption)
                {
                    selectedBindingOption = newSelectedBinding;
                    bindingId = bindingOptionValues[actionReference][newSelectedBinding];
                    bindingIdProperty.stringValue = bindingId;
                }
            }

            EditorGUI.EndProperty();
        }

        protected void RefreshBindingOptions(InputActionReference actionReference, string currentBindingId)
        {
            InputAction action = actionReference != null ? actionReference.action : null;

            if (action == null)
            {
                bindingOptions[actionReference] = new GUIContent[0];
                bindingOptionValues[actionReference] = new string[0];
                return;
            }

            ReadOnlyArray<InputBinding> bindings = action.bindings;
            int bindingCount = bindings.Count;

            bindingOptions[actionReference] = new GUIContent[bindingCount];
            bindingOptionValues[actionReference] = new string[bindingCount];

            for (var i = 0; i < bindingCount; ++i)
            {
                InputBinding binding = bindings[i];
                string bindingId = binding.id.ToString();
                bool hasBindingGroups = !string.IsNullOrEmpty(binding.groups);

                // If we don't have a binding groups (control schemes), show the device that if there are, for example,
                // there are two bindings with the display string "A", the user can see that one is for the keyboard
                // and the other for the gamepad.
                InputBinding.DisplayStringOptions displayOptions =
                    InputBinding.DisplayStringOptions.DontUseShortDisplayNames | 
                    InputBinding.DisplayStringOptions.IgnoreBindingOverrides;

                if (!hasBindingGroups)
                {
                    displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;
                }

                // Create display string.
                string displayString = action.GetBindingDisplayString(i, displayOptions);

                // If binding is part of a composite, include the part name.
                if (binding.isPartOfComposite)
                {
                    displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";
                }

                // Some composites use '/' as a separator. When used in popup, this will lead to to submenus. Prevent
                // by instead using a backlash.
                displayString = displayString.Replace('/', '\\');

                // If the binding is part of control schemes, mention them.
                if (hasBindingGroups)
                {
                    InputActionAsset asset = action.actionMap?.asset;

                    if (asset != null)
                    {
                        string controlSchemes = string.Join(", ", binding.groups.Split(InputBinding.Separator).Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name).NotNull());
                        displayString = $"{displayString} ({controlSchemes})";
                    }
                }

                bindingOptions[actionReference][i] = new GUIContent(displayString);
                bindingOptionValues[actionReference][i] = bindingId;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return rows * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
    }
}
