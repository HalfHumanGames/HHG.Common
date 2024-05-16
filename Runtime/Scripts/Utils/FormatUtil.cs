using System;

namespace HHG.Common.Runtime
{
    public static class FormatUtil
    {
        public static string Time(float seconds, bool minutes = true, bool hours = false)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return hours || time.Hours > 0 ? time.ToString(@"hh\:mm\:ss") : minutes || time.Minutes > 0 ? time.ToString(@"mm\:ss") : time.ToString(@"ss");
        }
    }
}