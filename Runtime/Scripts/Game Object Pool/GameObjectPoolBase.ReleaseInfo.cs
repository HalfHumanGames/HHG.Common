using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract partial class GameObjectPoolBase<T>
    {
        private struct ReleaseInfo
        {
            private enum ReleaseMode
            {
                Frames,
                Seconds
            }

            private T item;
            private int frameStamp;
            private float timeStamp;
            private bool unscaled;
            private ReleaseMode mode;

            public ReleaseInfo(T itemToRelease, int fameToRelease)
            {
                item = itemToRelease;
                frameStamp = fameToRelease;
                timeStamp = 0f;
                unscaled = false;
                mode = ReleaseMode.Frames;
            }

            public ReleaseInfo(T itemToRelease, float timeToRelease, bool unscaledTime = false)
            {
                item = itemToRelease;
                frameStamp = 0;
                timeStamp = timeToRelease;
                unscaled = unscaledTime;
                mode = ReleaseMode.Seconds;
            }

            public bool CanRelease(out T itemToRelease)
            {
                itemToRelease = item;
                return (mode == ReleaseMode.Frames && Time.frameCount >= frameStamp) || 
                       (mode == ReleaseMode.Seconds && (unscaled ? Time.unscaledTime : Time.time) >= timeStamp);
            }
        }
    } 
}