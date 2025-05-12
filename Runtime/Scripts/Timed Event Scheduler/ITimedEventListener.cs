namespace HHG.Common.Runtime
{
    public interface ITimedEventListener
    {
        public void OnTimedEventScheduled(TimedEvent timedEvent, float duration) { }
        public void OnTimedEventRescheduled(TimedEvent timedEvent, float duration) { }
        public void OnTimedEventUpdated(TimedEvent timedEvent, float timeElapsed) { }
        public void OnTimedEventUnscheduled(TimedEvent timedEvent) { }
        public void OnTimedEventExpired(TimedEvent timedEvent) { }
    }

    public interface ITimedEventListener<T> : ITimedEventListener
    {
        public void OnTimedEventScheduled(TimedEvent<T> timedEvent, float duration) { }
        public void OnTimedEventRescheduled(TimedEvent<T> timedEvent, float duration) { }
        public void OnTimedEventUpdated(TimedEvent<T> timedEvent, float timeElapsed) { }
        public void OnTimedEventUnscheduled(TimedEvent<T> timedEvent) { }
        public void OnTimedEventExpired(TimedEvent<T> timedEvent) { }
    }
}