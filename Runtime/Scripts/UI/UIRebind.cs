using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public class UIRebind : MonoBehaviour
    {
        [SerializeField] private GameObject rebindActionPrefab;
        [SerializeField] private GameObject rebindOverlay;
        [SerializeField] private GameObject keyboard;
        [SerializeField] private GameObject gamepad;
        [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions;
        [SerializeField] private List<InputActionBinding> keyboardBindings = new List<InputActionBinding>();
        [SerializeField] private List<InputActionBinding> gamepadBindings = new List<InputActionBinding>();

        private TMP_Text rebindLabel;

        private void Awake()
        {
            rebindLabel = rebindOverlay.GetComponentInChildren<TMP_Text>(true);
            rebindOverlay.SetActive(false);
            InitializeContainer(keyboard, keyboardBindings);
            InitializeContainer(gamepad, gamepadBindings);
        }

        private void InitializeContainer(GameObject container, List<InputActionBinding> bindings)
        {
            this.Destroy(container.GetChildren().Skip(1));

            foreach (InputActionBinding binding in bindings)
            {
                GameObject created = Instantiate(rebindActionPrefab, container.transform);
                created.name = rebindActionPrefab.name;

                if (created.TryGetComponent(out UIRebindAction rebindAction))
                {
                    rebindAction.ActionReference = binding.Action;
                    rebindAction.BindingId = binding.BindingId;
                    rebindAction.RebindOverlay = rebindOverlay;
                    rebindAction.RebindPrompt = rebindLabel;
                    rebindAction.DisplayStringOptions = displayStringOptions;
                }
            }
        }
    }
}
