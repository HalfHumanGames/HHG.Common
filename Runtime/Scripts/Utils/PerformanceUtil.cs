using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class PerformanceUtil
    {
        private static string label;
        private static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public static void MeasureDuration(string label, System.Action action)
        {
            sw.Restart();
            action();
            sw.Stop();
            Debug.Log($"{label}: {sw.ElapsedMilliseconds}ms");
        }

        public static void MeasureDurationStart(string label)
        {
            PerformanceUtil.label = label;
            sw.Restart();
        }

        public static void MeasureDurationStop()
        {
            sw.Stop();
            Debug.Log($"{label}: {sw.ElapsedMilliseconds}ms");
        }

        public static void MeasureAverageDuration(string label, System.Action action, int iterations = 1)
        {
            long sum = 0;
            for (int i = 0; i < iterations; i++) {
                sw.Restart();
                action();
                sw.Stop();
                sum += sw.ElapsedMilliseconds;
            }
            long avg = sum / iterations;
            Debug.Log($"{label}: {avg}ms");
        }
    }
}