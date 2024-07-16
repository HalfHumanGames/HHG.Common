using System;
using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class AudioSourceExtensions
    {
        public static Coroutine FadeTo(this AudioSource source, float target, float duration = 1f, Func<float, float> ease = null)
        {
            return CoroutineUtil.StartCoroutine(source.FadeToAsync(target, duration, ease));
        }

        public static IEnumerator FadeToAsync(this AudioSource source, float target, float duration = 1f, Func<float, float> ease = null)
        {
            yield return FadeToInternalAsync(source, 0f, target, duration, ease);
        }

        public static Coroutine FadeToDelayed(this AudioSource source, float delay, float target, float duration = 1f, Func<float, float> ease = null)
        {
            return CoroutineUtil.StartCoroutine(source.FadeToDelayedAsync(delay, target, duration, ease));
        }

        public static IEnumerator FadeToDelayedAsync(this AudioSource source, float delay, float target, float duration = 1f, Func<float, float> ease = null)
        {
            yield return FadeToInternalAsync(source, delay, target, duration, ease);
        }

        private static IEnumerator FadeToInternalAsync(AudioSource source, float delay, float target, float duration, Func<float, float> ease)
        {
            if (target > 0f && !source.isPlaying)
            {
                if (delay > 0f)
                {
                    source.PlayDelayed(delay);
                    yield return new WaitForSeconds(delay);
                }
                else
                {
                    source.Play();
                }
            }

            yield return TweenUtil.TweenAsync(() => source.volume, v => source.volume = v, target, duration, TimeScale.Unscaled, ease);

            if (target <= 0f)
            {
                source.Stop();
            }
        }
    }
}