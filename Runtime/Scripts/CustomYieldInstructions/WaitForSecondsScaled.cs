using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class WaitForSecondsScaled : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get
            {
                seconds -= Time.deltaTime * (getter?.Invoke() ?? timeScale);
                return seconds > 0f;
            }
        }

        private float seconds;
        private float timeScale;
        private Func<float> getter;

        public WaitForSecondsScaled(float secs, float scale)
        {
            seconds = secs;
            timeScale = scale;
        }

        public WaitForSecondsScaled(float secs, ref float scale)
        {
            seconds = secs;
            timeScale = scale;
        }

        public WaitForSecondsScaled(float secs, Func<float> scale)
        {
            seconds = secs;
            getter = scale;
        }
    }
}