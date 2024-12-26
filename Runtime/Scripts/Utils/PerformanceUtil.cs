using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace HHG.Common.Runtime
{
    public static class PerformanceUtil
    {
        public static void MeasureDuration(string label, System.Action action)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            Debug.Log($"{label}: {sw.ElapsedMilliseconds}ms");
        }
    }
}