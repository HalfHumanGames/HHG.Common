using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class SelectableStateHandler<T> : MonoBehaviour
    {
        [SerializeField] private StateValues<T> available;
        [SerializeField] private bool overrideUnavailable;
        [SerializeField] private StateValues<T> unavailable;

        private StateButton button;
        private T overrideValue;
        private bool hasOverride;

        protected abstract void Apply(T value, bool instant);

        // Lets game code force a value regardless of state until cleared
        public void SetOverride(T value)
        {
            overrideValue = value;
            hasOverride = true;
            OnStateChanged(false);
        }

        public void ClearOverride()
        {
            if (!hasOverride) return;

            hasOverride = false;
            OnStateChanged(false);
        }

        protected virtual void Awake()
        {
            button = GetComponentInParent<StateButton>(true);
        }

        protected virtual void OnEnable()
        {
            button.StateChanged += OnStateChanged;
            OnStateChanged(true);
        }

        protected virtual void OnDisable()
        {
            button.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(bool instant)
        {
            if (hasOverride)
            {
                Apply(overrideValue, instant);
                return;
            }

            StateValues<T> values = overrideUnavailable && !button.IsAvailable ? unavailable : available;
            Apply(values.Get(button.State), instant);
        }
    }
}
