namespace HHG.Common.Runtime
{
    public interface ITimedEventListener
    {
        void OnTimedEventScheduled(TimedEvent timedEvent, float duration) { }
        void OnTimedEventRescheduled(TimedEvent timedEvent, float duration) { }
        void OnTimedEventUpdate(TimedEvent timedEvent, float timeElapsed) { }
        void OnTimedEventUnscheduled(TimedEvent timedEvent) { }
        void OnTimedEventExpired(TimedEvent timedEvent) { }
    }

    public interface ITimedEventListener<T> : ITimedEventListener
    {
        void OnTimedEventScheduled(TimedEvent<T> timedEvent, float duration) { }
        void OnTimedEventRescheduled(TimedEvent<T> timedEvent, float duration) { }
        void OnTimedEventUpdate(TimedEvent<T> timedEvent, float timeElapsed) { }
        void OnTimedEventUnscheduled(TimedEvent<T> timedEvent) { }
        void OnTimedEventExpired(TimedEvent<T> timedEvent) { }
    }
}