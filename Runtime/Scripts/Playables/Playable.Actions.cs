using System;

namespace HHG.Common.Runtime
{
    public partial class Playable
    {
        private readonly struct Actions
        {
            public readonly Action<object> Play;
            public readonly Action<object> Stop;
            public readonly Action<object> Pause;
            public readonly Action<object> Resume;
            public readonly Func<object, bool> IsPlaying;

            public Actions(
                Action<object> play, 
                Action<object> stop = null, 
                Action<object> pause = null, 
                Action<object> resume = null,
                Func<object, bool> isPlaying = null)
            {
                Play = play;
                Stop = stop;
                Pause = pause;
                Resume = resume;
                IsPlaying = isPlaying;
            }
        }
    }
}
