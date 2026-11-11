using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    // A Button that reports its visual state so separate components can react to it.
    // Unavailable buttons stay selectable (unlike non-interactable ones) but can be styled as unusable.
    public class StateButton : Button
    {
        public SelectableState State => state;

        public bool IsAvailable
        {
            get => isAvailable;
            set
            {
                if (isAvailable == value) return;

                isAvailable = value;
                StateChanged?.Invoke(false);
            }
        }

        // Arg: whether the change should apply instantly
        public event System.Action<bool> StateChanged;

        private SelectableState state;
        private bool isAvailable = true;
        private bool hasState;

        protected override void DoStateTransition(SelectionState selectionState, bool instant)
        {
            base.DoStateTransition(selectionState, instant);

            SelectableState newState = (SelectableState)selectionState;

            // Unity re-sends the current state instantly on enable, which listeners use to snap into place
            if (hasState && newState == state && !instant) return;

            hasState = true;
            state = newState;
            StateChanged?.Invoke(instant);
        }
    }
}
