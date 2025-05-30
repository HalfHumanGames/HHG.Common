using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class PerformanceUtil
    {
        private static string label = string.Empty;
        private static int maxMS = int.MaxValue;
        private static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public static void MeasureDuration(string label, System.Action action, int maxMS = int.MaxValue)
        {
            PerformanceUtil.label = label;
            PerformanceUtil.maxMS = maxMS;
            sw.Restart();
            action();
            sw.Stop();
            Log(sw.ElapsedMilliseconds);
        }

        public static void MeasureDurationStart(string label, int maxMS = int.MaxValue)
        {
            PerformanceUtil.label = label;
            PerformanceUtil.maxMS = maxMS;
            sw.Restart();
        }

        public static void MeasureDurationStop()
        {
            sw.Stop();
            Log(sw.ElapsedMilliseconds);
        }

        public static void MeasureAverageDuration(string label, System.Action action, int iterations = 1, int maxMS = int.MaxValue)
        {
            PerformanceUtil.label = label;
            PerformanceUtil.maxMS = maxMS;
            long sum = 0;
            for (int i = 0; i < iterations; i++) {
                sw.Restart();
                action();
                sw.Stop();
                sum += sw.ElapsedMilliseconds;
            }
            long avg = sum / iterations;
            Log(avg);
        }

        private static void Log(long ms)
        {
            string message = $"{label}: {ms}ms";
            if (ms >= maxMS)
            {
                Debug.LogError(message);
            }
            else
            {
                Debug.Log(message);
            }
        }
    }
}