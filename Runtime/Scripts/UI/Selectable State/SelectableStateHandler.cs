using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class SelectableStateHandler<T> : MonoBehaviour
    {
        [SerializeField] private StateButton button;
        [SerializeField] private StateValues<T> available;
        [SerializeField] private bool overrideUnavailable;
        [SerializeField] private StateValues<T> unavailable;

        protected abstract void Apply(T value, bool instant);

        protected virtual void Reset()
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
            StateValues<T> values = overrideUnavailable && !button.IsAvailable ? unavailable : available;
            Apply(values.Get(button.State), instant);
        }
    }
}
