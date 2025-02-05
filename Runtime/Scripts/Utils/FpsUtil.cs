using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class FpsUtil
    {
        private const float smoothingFactor = .1f;

        public static void UpdateFps(ref float fps)
        {
            // Exponentially weighted moving average (EWMA) needs to update each frame
            // https://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average
            fps = (smoothingFactor * (1f / Time.unscaledDeltaTime)) + ((1f - smoothingFactor) * fps);
        }
    }
}