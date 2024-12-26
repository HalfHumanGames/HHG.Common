using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class DebugUtil
    {
        public static void LogException(string message, Object context = null)
        {
            Debug.LogException(new System.Exception(message), context);
        }
    }
}