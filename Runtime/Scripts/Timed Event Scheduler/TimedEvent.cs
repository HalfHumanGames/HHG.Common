using System;

namespace HHG.Common.Runtime
{
    public class TimedEvent : IComparable<TimedEvent>
    {
        public object Context => context;
        public bool IsExpired => timeRemaining <= 0f;
        public float TimeRemaining => timeRemaining;

        private object context;
        private float timeRemaining;

        public event Action<TimedEvent> Expired;
        public event Action<TimedEvent> Rescheduled;

        public TimedEvent(float timeRemaining = 0f, object ctx = default)
        {
            Initialize(timeRemaining, ctx);
        }

        // In case you want to reinitialize a
        // timed event in order to reuse it
        public void Initialize(float duration = 0f, object ctx = default)
        {
            timeRemaining = duration;
            context = ctx;
            Expired = null;
            Rescheduled = null;
        }

        public void Update(float timeElapsed)
        {
            // Use field since we do not want
            // to trigger TimeRemainingUpdated
            timeRemaining -= timeElapsed;
        }

        public void Reschedule(float duration)
        {
            if (timeRemaining != duration)
            {
                timeRemaining = duration;
                Rescheduled?.Invoke(this);
            }
        }

        public void Expire()
        {
            Expired?.Invoke(this);
        }

        public int CompareTo(TimedEvent other)
        {
            return timeRemaining.CompareTo(other.timeRemaining);
        }
    }
}