using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class PerformanceUtil
    {
        public static void MeasureDuration(string label, System.Action action)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            Debug.Log($"{label}: {sw.ElapsedMilliseconds}ms");
        }

        public static void MeasureAverageDuration(string label, System.Action action, int iterations = 1)
        {
            long sum = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
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