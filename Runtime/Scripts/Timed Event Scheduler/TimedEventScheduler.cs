using System.Collections.Generic;
using System.Text;

namespace HHG.Common.Runtime
{
    public class TimedEventScheduler
    {
        public IReadOnlyList<TimedEvent> ScheduledEvents => scheduledEvents;
        public IReadOnlyList<TimedEvent> ExpiredEvents => expiredEvents;

        private HashSet<TimedEvent> scheduledEventsHash = new HashSet<TimedEvent>();
        private HashSet<TimedEvent> expiredEventsHash = new HashSet<TimedEvent>();
        private List<TimedEvent> scheduledEvents = new List<TimedEvent>();
        private List<TimedEvent> expiredEvents = new List<TimedEvent>();

        public bool IsScheduled(TimedEvent timedEvent) => scheduledEventsHash.Contains(timedEvent);
        public bool IsExpired(TimedEvent timedEvent) => expiredEventsHash.Contains(timedEvent);

        public void Schedule(TimedEvent timedEvent)
        {
            if (!scheduledEventsHash.Contains(timedEvent))
            {
                timedEvent.Schedule(timedEvent.TimeRemaining);
                timedEvent.WeakRescheduled += OnRescheduled;
                scheduledEvents.SortedInsert(timedEvent);
                scheduledEventsHash.Add(timedEvent);
            }
        }

        public void Unschedule(TimedEvent timedEvent)
        {
            if (scheduledEventsHash.Contains(timedEvent))
            {
                timedEvent.Unschedule();
                timedEvent.WeakRescheduled -= OnRescheduled;
                scheduledEvents.Remove(timedEvent);
                scheduledEventsHash.Remove(timedEvent);
            }

            if (expiredEventsHash.Contains(timedEvent))
            {
                expiredEvents.Remove(timedEvent);
                expiredEventsHash.Remove(timedEvent);
            }
        }

        public void Update(float deltaTime)
        {
            if (scheduledEvents.Count == 0)
            {
                return;
            }

            for (int i = 0; i < scheduledEvents.Count; i++)
            {
                TimedEvent scheduledEvent = scheduledEvents[i];
                scheduledEvent.Update(deltaTime);

                if (scheduledEvent.IsExpired)
                {
                    scheduledEvent.WeakRescheduled -= OnRescheduled;
                    scheduledEvents.RemoveAt(i--);
                    scheduledEventsHash.Remove(scheduledEvent);
                    expiredEvents.Add(scheduledEvent);
                    expiredEventsHash.Add(scheduledEvent);
                }
            }
        }

        public void ExpireAll()
        {
            while (expiredEvents.Count > 0)
            {
                ExpireNext();
            }
        }

        public bool ExpireNext()
        {
            return ExpireNext(out _);
        }

        public bool ExpireNext(out TimedEvent expiredEvent)
        {
            if (expiredEvents.Count > 0)
            {
                expiredEvent = expiredEvents[0];
                expiredEvent.Expire();
                expiredEvents.RemoveAt(0);
                expiredEventsHash.Remove(expiredEvent);
                return true;
            }
            else
            {
                expiredEvent = null;
                return false;
            }  
        }

        public void Clear()
        {
            foreach (TimedEvent timedEvent in scheduledEvents)
            {
                timedEvent.WeakRescheduled -= OnRescheduled;
            }

            scheduledEvents.Clear();
            scheduledEventsHash.Clear();
            expiredEvents.Clear();
            expiredEventsHash.Clear();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("TimedEventScheduler");
            foreach (TimedEvent timedEvent in scheduledEvents)
            {
                sb.AppendLine($" - {timedEvent}");
            }
            return sb.ToString();
        }

        private void OnRescheduled(TimedEvent timedEvent, float duration)
        {
            scheduledEvents.ResortItem(timedEvent);
        }
    }
}
