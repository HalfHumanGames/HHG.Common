using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public struct StateValues<T>
    {
        [SerializeField] private T normal;
        [SerializeField] private T highlighted;
        [SerializeField] private T pressed;
        [SerializeField] private T selected;
        [SerializeField] private T disabled;

        public T Get(SelectableState state)
        {
            switch (state)
            {
                case SelectableState.Highlighted: return highlighted;
                case SelectableState.Pressed: return pressed;
                case SelectableState.Selected: return selected;
                case SelectableState.Disabled: return disabled;
                default: return normal;
            }
        }
    }
}
