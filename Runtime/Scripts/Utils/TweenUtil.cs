using System;
using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class TweenUtil
    {
        public static IEnumerator TweenAsync(Func<float> getter, Action<float> setter, float end, float duration, Func<float, float> ease = null)
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
                yield return WaitFor.EndOfFrame;
            }

            setter(end);
        }
    }
}