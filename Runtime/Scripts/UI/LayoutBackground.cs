using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    // Sizes this RectTransform to cover its laid-out siblings plus padding, or to zero when there are none.
    // It excludes itself from the parent's layout, so it works under any layout group.
    // Centered pivot, so the Transform's scale grows it evenly.
    [ExecuteAlways, RequireComponent(typeof(RectTransform))]
    public class LayoutBackground : UIBehaviour, ILayoutSelfController, ILayoutIgnorer
    {
        public bool ignoreLayout => true;

        [SerializeField] private RectOffset padding = new RectOffset();

        private RectTransform rect => (RectTransform)transform;
        private DrivenRectTransformTracker tracker;

        public void SetLayoutHorizontal() { }

        // Runs after the parent's layout has placed the siblings on both axes
        public void SetLayoutVertical()
        {
            tracker.Clear();
            tracker.Add(this, rect,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.Pivot |
                DrivenTransformProperties.AnchoredPosition |
                DrivenTransformProperties.SizeDelta);

            Vector2 center = new Vector2(.5f, .5f);
            rect.anchorMin = center;
            rect.anchorMax = center;
            rect.pivot = center;

            RectTransform parent = rect.parent as RectTransform;
            Vector2 min = Vector2.positiveInfinity;
            Vector2 max = Vector2.negativeInfinity;

            if (parent != null)
            {
                foreach (Transform child in parent)
                {
                    if (child == transform || !child.gameObject.activeSelf || child is not RectTransform sibling) continue;
                    if (child.TryGetComponent(out ILayoutIgnorer ignorer) && ignorer.ignoreLayout) continue;

                    Vector2 position = sibling.localPosition;
                    Vector2 scale = sibling.localScale;
                    min = Vector2.Min(min, position + Vector2.Scale(sibling.rect.min, scale));
                    max = Vector2.Max(max, position + Vector2.Scale(sibling.rect.max, scale));
                }
            }

            if (min.x > max.x)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
                return;
            }

            min -= new Vector2(padding.left, padding.bottom);
            max += new Vector2(padding.right, padding.top);

            rect.anchoredPosition = (min + max) * .5f - parent.rect.center;
            rect.sizeDelta = max - min;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            tracker.Clear();
            SetDirty();
            base.OnDisable();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif

        private void SetDirty()
        {
            if (transform.parent is RectTransform parent)
            {
                LayoutRebuilder.MarkLayoutForRebuild(parent);
            }
        }
    }
}
