using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public abstract class TimeEffect
    {
        public abstract IEnumerator Execute();

        [System.Serializable]
        public class Wait : TimeEffect
        {
            [SerializeField] private float duration = 0.1f;

            public override IEnumerator Execute()
            {
                yield return new WaitForSecondsRealtime(duration);
            }
        }

        [System.Serializable]
        public class FreezeFrame : TimeEffect
        {
            [SerializeField] private float duration = 0.08f;

            public override IEnumerator Execute()
            {
                Time.timeScale = 0f;
                yield return new WaitForSecondsRealtime(duration);
            }
        }

        [System.Serializable]
        public class Hitstop : TimeEffect
        {
            [Range(0.01f, 0.3f)][SerializeField] private float scale = 0.1f;
            [SerializeField] private float duration = 0.15f;

            public override IEnumerator Execute()
            {
                Time.timeScale = scale;
                yield return new WaitForSecondsRealtime(duration);
            }
        }

        [System.Serializable]
        public class SlowMotion : TimeEffect
        {
            [Range(0.05f, 0.5f)][SerializeField] private float targetScale = 0.2f;
            [SerializeField] private float holdDuration = 1.5f;
            [SerializeField] private float transitionDuration = 0.3f;

            public override IEnumerator Execute()
            {
                float startTime = Time.realtimeSinceStartup;
                while (true)
                {
                    float t = (Time.realtimeSinceStartup - startTime) / transitionDuration;
                    if (t >= 1f) break;
                    Time.timeScale = Mathf.SmoothStep(1f, targetScale, t);
                    yield return new WaitForEndOfFrame();
                }

                Time.timeScale = targetScale;
                yield return new WaitForSecondsRealtime(holdDuration);

                startTime = Time.realtimeSinceStartup;
                while (true)
                {
                    float t = (Time.realtimeSinceStartup - startTime) / transitionDuration;
                    if (t >= 1f) break;
                    Time.timeScale = Mathf.SmoothStep(targetScale, 1f, t);
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        [System.Serializable]
        public class AnticipationRamp : TimeEffect
        {
            [Range(1.1f, 2f)][SerializeField] private float fastScale = 1.4f;
            [SerializeField] private float fastDuration = 0.1f;
            [SerializeField] private float freezeDuration = 0.08f;

            public override IEnumerator Execute()
            {
                Time.timeScale = fastScale;
                yield return new WaitForSecondsRealtime(fastDuration);
                Time.timeScale = 0f;
                yield return new WaitForSecondsRealtime(freezeDuration);
            }
        }
    }
}
