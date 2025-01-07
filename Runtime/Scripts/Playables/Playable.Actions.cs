using System;

namespace HHG.Common.Runtime
{
    public partial class Playable
    {
        private struct Actions
        {
            public readonly Action<object> Play;
            public readonly Action<object> Stop;
            public readonly Action<object> Pause;
            public readonly Action<object> Resume;

            public Actions(Action<object> play, Action<object> stop = null, Action<object> pause = null, Action<object> resune = null)
            {
                Play = play;
                Stop = stop;
                Pause = pause;
                Resume = resune;
            }
        }
    }
}
