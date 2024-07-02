using System;
using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class TweenUtil
    {
        public static IEnumerator TweenAsync(Func<Vector2> getter, Action<Vector2> setter, Vector2 end, float duration, Func<float, float> ease = null, Action onUpdate = null, Action onComplete = null)
        {
            float elapsed = 0f;
            Vector2 start = getter();
            ease ??= EaseUtil.Linear;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float easedT = ease(t);
                Vector2 newValue = Vector2.Lerp(start, end, easedT);
                setter(newValue);
                onUpdate?.Invoke();
                yield return WaitFor.EndOfFrame;
            }

            setter(end);
            onComplete?.Invoke();
        }

        public static IEnumerator TweenAsync(Func<Vector3> getter, Action<Vector3> setter, Vector3 end, float duration, Func<float, float> ease = null, Action onUpdate = null, Action onComplete = null)
        {
            float elapsed = 0f;
            Vector3 start = getter();
            ease ??= EaseUtil.Linear;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float easedT = ease(t);
                Vector3 newValue = Vector3.Lerp(start, end, easedT);
                setter(newValue);
                onUpdate?.Invoke();
                yield return WaitFor.EndOfFrame;
            }

            setter(end);
            onComplete?.Invoke();
        }

        public static IEnumerator TweenAsync(Func<Color> getter, Action<Color> setter, Color end, float duration, Func<float, float> ease = null, Action onUpdate = null, Action onComplete = null)
        {
            float elapsed = 0f;
            Color start = getter();
            ease ??= EaseUtil.Linear;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float easedT = ease(t);
                Color newValue = Color.Lerp(start, end, easedT);
                setter(newValue);
                onUpdate?.Invoke();
                yield return WaitFor.EndOfFrame;
            }

            setter(end);
            onComplete?.Invoke();
        }

        public static IEnumerator TweenAsync(Func<float> getter, Action<float> setter, float end, float duration, Func<float, float> ease = null, Action onUpdate = null, Action onComplete = null)
        {
            float elapsed = 0f;
            float start = getter();
            ease ??= EaseUtil.Linear;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float easedT = ease(t);
                float newValue = Mathf.Lerp(start, end, easedT);
                setter(newValue);
                onUpdate?.Invoke();
                yield return WaitFor.EndOfFrame;
            }

            setter(end);
            onComplete?.Invoke();
        }
    }
}