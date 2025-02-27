using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace HHG.Common.Runtime
{
    public partial class UIRebindAction : MonoBehaviour
    {
        [Serializable] public class UpdateBindingUIEvent : UnityEvent<UIRebindAction, string, string, string> { }
        [Serializable] public class InteractiveRebindEvent : UnityEvent<UIRebindAction, InputActionRebindingExtensions.RebindingOperation> { }

        public InputActionReference ActionReference
        {
            get => actionReference;
            set
            {
                actionReference = value;
                UpdateActionLabel();
                UpdateBindingDisplay();
            }
        }

        public string BindingId
        {
            get => bindingId;
            set
            {
                bindingId = value;
                UpdateBindingDisplay();
            }
        }

        public InputBinding.DisplayStringOptions DisplayStringOptions
        {
            get => displayStringOptions;
            set
            {
                displayStringOptions = value;
                UpdateBindingDisplay();
            }
        }

        public TMP_Text ActionLabel
        {
            get => actionLabel;
            set
            {
                actionLabel = value;
                UpdateActionLabel();
            }
        }

        public TMP_Text BindingText
        {
            get => bindingText;
            set
            {
                bindingText = value;
                UpdateBindingDisplay();
            }
        }

        public TMP_Text RebindPrompt
        {
            get => rebindText;
            set => rebindText = value;
        }

        public GameObject RebindOverlay
        {
            get => rebindOverlay;
            set => rebindOverlay = value;
        }

        public UpdateBindingUIEvent OnUpdateBinding
        {
            get
            {
                onUpdateBinding ??= new UpdateBindingUIEvent();
                return onUpdateBinding;
            }
        }

        public InteractiveRebindEvent OnStartRebind
        {
            get
            {
                onStartRebind ??= new InteractiveRebindEvent();
                return onStartRebind;
            }
        }

        public InteractiveRebindEvent OnStopRebind
        {
            get
            {
                onStopRebind ??= new InteractiveRebindEvent();
                return onStopRebind;
            }
        }

        public InputActionRebindingExtensions.RebindingOperation ongoingRebind => rebindOperation;

        // Keep these fields for now so that they work with previous implementations
        [SerializeField, HideInInspector, FormerlySerializedAs("m_Action")] private InputActionReference _actionReference;
        [SerializeField, HideInInspector, FormerlySerializedAs("m_BindingId")] private string _bindingId;

        [SerializeField] private InputActionBinding binding;
        [SerializeField, FormerlySerializedAs("m_DisplayStringOptions")] private InputBinding.DisplayStringOptions displayStringOptions;
        [SerializeField, FormerlySerializedAs("m_ActionLabel")] private TMP_Text actionLabel;
        [SerializeField, FormerlySerializedAs("m_BindingText")] private TMP_Text bindingText;
        [SerializeField, FormerlySerializedAs("m_RebindOverlay")] private GameObject rebindOverlay;
        [SerializeField, FormerlySerializedAs("m_RebindText")] private TMP_Text rebindText;
        [SerializeField, FormerlySerializedAs("m_UpdateBindingUIEvent")] private UpdateBindingUIEvent onUpdateBinding;
        [SerializeField, FormerlySerializedAs("m_RebindStartEvent")] private InteractiveRebindEvent onStartRebind;
        [SerializeField, FormerlySerializedAs("m_RebindStopEvent")] private InteractiveRebindEvent onStopRebind;

        private InputActionRebindingExtensions.RebindingOperation rebindOperation;

        private InputActionReference actionReference
        {
            get => binding?.Action;
            set => binding.Action = value;
        }

        private string bindingId
        {
            get => binding?.BindingId;
            set => binding.BindingId = value;
        }

        private void OnEnable()
        {
            FixFields();

            rebindActions ??= new List<UIRebindAction>();
            rebindActions.Add(this);

            if (rebindActions.Count == 1)
            {
                InputSystem.onActionChange += OnActionChange;
            }
        }

        private void OnDisable()
        {
            rebindOperation?.Dispose();
            rebindOperation = null;
            rebindActions.Remove(this);

            if (rebindActions.Count == 0)
            {
                rebindActions = null;
                InputSystem.onActionChange -= OnActionChange;
            }
        }

        public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;
            action = actionReference != null ? actionReference.action : null;

            if (action == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(bindingId))
            {
                return false;
            }

            // Look up binding index.
            Guid bindingGuid = new Guid(bindingId);
            bindingIndex = action.bindings.IndexOf(x => x.id == bindingGuid);

            if (bindingIndex == -1)
            {
                Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
                return false;
            }

            return true;
        }

        public void UpdateActionLabel()
        {
            if (actionLabel != null)
            {
                InputAction action = actionReference != null ? actionReference.action : null;
                actionLabel.text = action != null ? action.name : string.Empty;
            }
        }

        public void UpdateBindingDisplay()
        {
            string displayString = string.Empty;
            string deviceLayoutName = default;
            string controlPath = default;

            // Get display string from action.
            InputAction action = actionReference != null ? actionReference.action : null;
            if (action != null)
            {
                int bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == bindingId);

                if (bindingIndex != -1)
                {
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, DisplayStringOptions);
                }
            }

            // Set on label (if any).
            if (bindingText != null)
            {
                bindingText.text = displayString;
            }

            // Give listeners a chance to configure UI in response.
            onUpdateBinding?.Invoke(this, displayString, deviceLayoutName, controlPath);
        }

        public void ResetToDefault()
        {
            if (!ResolveActionAndBinding(out InputAction action, out int bindingIndex))
            {
                return;
            }

            if (action.bindings[bindingIndex].isComposite)
            {
                // It's a composite. Remove overrides from part bindings.
                for (int i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                {
                    action.RemoveBindingOverride(i);
                }
            }
            else
            {
                action.RemoveBindingOverride(bindingIndex);
            }

            UpdateBindingDisplay();
        }

        public void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out InputAction action, out int bindingIndex))
            {
                return;
            }

            // If the binding is a composite, we need to rebind each part in turn.
            if (action.bindings[bindingIndex].isComposite)
            {
                int firstPartIndex = bindingIndex + 1;

                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                {
                    PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
                }
            }
            else
            {
                PerformInteractiveRebind(action, bindingIndex);
            }
        }

        private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            rebindOperation?.Cancel(); // Will null out m_RebindOperation.

            //Fixes the "InvalidOperationException: Cannot rebind action x while it is enabled" error
            action.Disable();

            // Configure the rebind.
            rebindOperation = action.PerformInteractiveRebinding(bindingIndex).
                OnCancel(operation =>
                {
                    onStopRebind?.Invoke(this, operation);

                    if (rebindOverlay != null)
                    {
                        rebindOverlay.SetActive(false);
                    }

                    UpdateBindingDisplay();
                    CleanUp();
                }).
                OnComplete(operation =>
                {
                    if (rebindOverlay != null)
                    {
                        rebindOverlay.SetActive(false);
                    }

                    onStopRebind?.Invoke(this, operation);
                    UpdateBindingDisplay();
                    CleanUp();

                    // If there's more composite parts we should bind, initiate a rebind
                    // for the next part.
                    if (allCompositeParts)
                    {
                        int nextBindingIndex = bindingIndex + 1;

                        if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                        {
                            PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                    }
                });

            // If it's a part binding, show the name of the part in the UI.
            string partName = default;
            if (action.bindings[bindingIndex].isPartOfComposite)
            {
                partName = $"Binding '{action.bindings[bindingIndex].name}'. ";
            }

            // Bring up rebind overlay, if we have one.
            rebindOverlay?.SetActive(true);
            if (rebindText != null)
            {
                string text = !string.IsNullOrEmpty(rebindOperation.expectedControlType)
                    ? $"{partName}Waiting for {rebindOperation.expectedControlType} input..."
                    : $"{partName}Waiting for input...";
                rebindText.text = text;
            }

            // If we have no rebind overlay and no callback but we have a binding TMP_Text label,
            // temporarily set the binding TMP_Text label to "<Waiting>".
            if (rebindOverlay == null && rebindText == null && onStartRebind == null && bindingText != null)
            {
                bindingText.text = "<Waiting...>";
            }

            // Give listeners a chance to act on the rebind starting.
            onStartRebind?.Invoke(this, rebindOperation);

            rebindOperation.Start();

            void CleanUp()
            {
                rebindOperation?.Dispose();
                rebindOperation = null;
                action.Enable();
            }
        }

        private void FixFields()
        {
            binding ??= new InputActionBinding();

            if (_actionReference != null)
            {
                binding.Action = _actionReference;
                _actionReference = null;
            }

            if (!string.IsNullOrEmpty(_bindingId))
            {
                binding.BindingId = _bindingId;
                _bindingId = null;
            }
        }

        private void OnValidate()
        {
            FixFields();
            UpdateActionLabel();
            UpdateBindingDisplay();
        }
    }
}
