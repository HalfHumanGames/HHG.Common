using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class WaitFor
    {
        public static readonly WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate FixedUpdate = new WaitForFixedUpdate();
    }
}