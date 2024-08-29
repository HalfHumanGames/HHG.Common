using System;
using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class TweenUtil
    {
        public static IEnumerator TweenAsync(Func<Vector2> getter, Action<Vector2> setter, Vector2 end, float duration, TimeScale timeScale = TimeScale.Scaled, Func<float, float> ease = null, Action onUpdate = null, Action onComplete = null)
        {
            return TweenAsyncInternal(getter, setter, end, duration, timeScale, ease, onUpdate, onComplete, Vector2.Lerp);
        }

        public static IEnumerator TweenAsync(Func<Vector3> getter, Action<Vector3> setter, Vector3 end, float duration, TimeScale timeScale = TimeScale.Scaled, Func<float, float> ease = null, Action onUpdate = null, Action onComplete = null)
        {
            return TweenAsyncInternal(getter, setter, end, duration, timeScale, ease, onUpdate, onComplete, Vector3.Lerp);
        }

        public static IEnumerator TweenAsync(Func<Color> getter, Action<Color> setter, Color end, float duration, TimeScale timeScale = TimeScale.Scaled, Func<float, float> ease = null, Action onUpdate = null, Action onComplete = null)
        {
            return TweenAsyncInternal(getter, setter, end, duration, timeScale, ease, onUpdate, onComplete, Color.Lerp);
        }

        public static IEnumerator TweenAsync(Func<float> getter, Action<float> setter, float end, float duration, TimeScale timeScale = TimeScale.Scaled, Func<float, float> ease = null, Action onUpdate = null, Action onComplete = null)
        {
            return TweenAsyncInternal(getter, setter, end, duration, timeScale, ease, onUpdate, onComplete, Mathf.Lerp);
        }

        public static IEnumerator TweenAsyncInternal<T>(Func<T> getter, Action<T> setter, T end, float duration, TimeScale timeScale, Func<float, float> ease, Action onUpdate, Action onComplete, Func<T, T, float, T> lerp)
        {
            float elapsed = 0f;
            T start = getter();
            ease ??= EaseUtil.Linear;

            while (elapsed < duration)
            {
                elapsed += timeScale == TimeScale.Scaled ? Time.deltaTime : Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float easedT = ease(t);
                T newValue = lerp(start, end, easedT);
                setter(newValue);
                onUpdate?.Invoke();
                yield return new WaitForEndOfFrame();
            }

            setter(end);
            onComplete?.Invoke();
        }
    }
}