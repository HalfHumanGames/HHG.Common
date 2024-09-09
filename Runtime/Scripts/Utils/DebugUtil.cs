using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HHG.Common.Runtime
{
    public static class DebugUtil
    {
        public static void LogException(string message, Object context = null)
        {
            Debug.LogException(new Exception(message), context);
        }
    }
}