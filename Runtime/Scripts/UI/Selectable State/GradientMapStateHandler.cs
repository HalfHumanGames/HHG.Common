using UnityEngine;

namespace HHG.Common.Runtime
{
    public class GradientMapStateHandler : SelectableStateHandler<GradientMapRamp>
    {
        [SerializeField, Min(0f)] private float fadeDuration = .1f;

        private UIGradientMap[] targets = new UIGradientMap[0];

        protected override void Awake()
        {
            base.Awake();
            targets = GetComponentsInChildren<UIGradientMap>(true);
        }

        protected override void Apply(GradientMapRamp ramp, bool instant)
        {
            foreach (UIGradientMap target in targets)
            {
                target.SetRamp(ramp, instant ? 0f : fadeDuration);
            }
        }
    }
}
