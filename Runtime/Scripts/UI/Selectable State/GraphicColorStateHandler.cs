using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    // Sets a Graphic's color (not the CanvasRenderer tint) per state, so text keeps exact authored colors
    public class GraphicColorStateHandler : SelectableStateHandler<Color>
    {
        [SerializeField] private Graphic target;
        [SerializeField, Min(0f)] private float fadeDuration = .1f;

        private Coroutine fade;

        protected override void Reset()
        {
            base.Reset();
            target = GetComponent<Graphic>();
        }

        protected override void Apply(Color color, bool instant)
        {
            if (fade != null)
            {
                StopCoroutine(fade);
                fade = null;
            }

            if (instant || fadeDuration <= 0f || !isActiveAndEnabled)
            {
                target.color = color;
            }
            else
            {
                fade = StartCoroutine(FadeAsync(color));
            }
        }

        private IEnumerator FadeAsync(Color color)
        {
            Color start = target.color;

            for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / fadeDuration)
            {
                target.color = Color.LerpUnclamped(start, color, t);
                yield return null;
            }

            target.color = color;
            fade = null;
        }
    }
}
