using UnityEngine;

namespace HHG.Common.Runtime
{
    public class Require : MonoBehaviour
    {
        [SerializeField] private UpdateMode updateMode;
        [SerializeReference, ReferencePicker] private IRequirement requirements;
        [SerializeReference, ReferencePicker] private ISubscribable updateTriggers;

        [System.Flags]
        private enum UpdateMode
        {
            Manual = 0,
            Awake = 1 << 0,
            Start = 1 << 1,
            Triggers = 1 << 2
        }

        private void Awake()
        {
            if (updateMode.HasFlag(UpdateMode.Awake))
            {
                UpdateVisibility();
            }

            if (updateMode.HasFlag(UpdateMode.Triggers))
            {
                updateTriggers?.Subscribe(UpdateVisibility);
            }
        }

        private void Start()
        {
            if (updateMode.HasFlag(UpdateMode.Start))
            {
                UpdateVisibility();
            }
        }

        public void UpdateVisibility()
        {
            bool isVisible = requirements == null || requirements.IsRequirementMet(this);
            gameObject.SetActive(isVisible);
        }

        private void OnDestroy()
        {
            updateTriggers?.Unsubscribe(UpdateVisibility);
        }
    }
}