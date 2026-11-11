using UnityEngine;

namespace HHG.Common.Runtime
{
    public class GradientMapStateHandler : SelectableStateHandler<int>
    {
        [SerializeField] private UIGradientMap[] targets = new UIGradientMap[0];
        [SerializeField, Min(0f)] private float fadeDuration = .1f;

        protected override void Reset()
        {
            base.Reset();
            targets = GetComponentsInChildren<UIGradientMap>(true);
        }

        protected override void Apply(int row, bool instant)
        {
            foreach (UIGradientMap target in targets)
            {
                target.SetRow(row, instant ? 0f : fadeDuration);
            }
        }
    }
}
