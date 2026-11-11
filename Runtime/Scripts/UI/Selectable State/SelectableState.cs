namespace HHG.Common.Runtime
{
    // Mirrors Selectable.SelectionState (which is protected) in the same order so the two cast directly
    public enum SelectableState
    {
        Normal,
        Highlighted,
        Pressed,
        Selected,
        Disabled
    }
}
