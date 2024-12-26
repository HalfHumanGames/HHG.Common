using System.Collections.Generic;

namespace HHG.Common.Runtime
{
    public class TimedEventComparer : IComparer<TimedEvent>
    {
        public static readonly TimedEventComparer Instance = new TimedEventComparer();

        public int Compare(TimedEvent a, TimedEvent b)
        {
            return a.TimeRemaining.CompareTo(b.TimeRemaining);
        }
    }
}