using System;
using System.Diagnostics;

namespace HHG.Common.Runtime
{
    public static class PerformanceUtil
    {
        public static void MeasureDuration(string label, Action action)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            UnityEngine.Debug.Log($"{label}: {sw.ElapsedMilliseconds}ms");
        }
    }
}