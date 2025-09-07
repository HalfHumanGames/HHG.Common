namespace HHG.Common.Runtime
{
    public interface ITimedEventListener
    {
        public void OnTimedEventSchedule(TimedEvent timedEvent, float duration) { }
        public void OnTimedEventReschedule(TimedEvent timedEvent, float duration) { }
        public void OnTimedEventUpdate(TimedEvent timedEvent, float timeElapsed) { }
        public void OnTimedEventLateUpdate(TimedEvent timedEvent, float timeElapsed) { }
        public void OnTimedEventUnscheduled(TimedEvent timedEvent) { }
        public void OnTimedEventExpire(TimedEvent timedEvent) { }
    }

    public interface ITimedEventListener<T> : ITimedEventListener
    {
        public void OnTimedEventSchedule(TimedEvent<T> timedEvent, float duration) { }
        public void OnTimedEventReschedule(TimedEvent<T> timedEvent, float duration) { }
        public void OnTimedEventUpdate(TimedEvent<T> timedEvent, float timeElapsed) { }
        public void OnTimedEventLateUpdate(TimedEvent<T> timedEvent, float timeElapsed) { }
        public void OnTimedEventUnschedule(TimedEvent<T> timedEvent) { }
        public void OnTimedEventExpire(TimedEvent<T> timedEvent) { }
    }
}