using System.Collections.Generic;

namespace HHG.Common.Runtime
{
    // Could further optimize by using a priority queue instead of list.sort
    public class TimedEventScheduler
    {
        public IReadOnlyList<TimedEvent> ScheduledEvents => scheduledEvents;
        public IReadOnlyList<TimedEvent> ExpiredEvents => expiredEvents;

        private HashSet<TimedEvent> scheduledEventsHash = new HashSet<TimedEvent>();
        private List<TimedEvent> scheduledEvents = new List<TimedEvent>();
        private List<TimedEvent> expiredEvents = new List<TimedEvent>();
        private bool isDirty = false;

        public void Schedule(TimedEvent timedEvent)
        {
            if (!scheduledEventsHash.Contains(timedEvent))
            {
                timedEvent.Rescheduled += OnRescheduled;
                scheduledEvents.Add(timedEvent);
                scheduledEventsHash.Add(timedEvent);
                isDirty = true;
            }
        }

        public void Unschedule(TimedEvent timedEvent)
        {
            if (scheduledEventsHash.Contains(timedEvent))
            {
                timedEvent.Rescheduled -= OnRescheduled;
                scheduledEvents.Remove(timedEvent);
                scheduledEventsHash.Remove(timedEvent);
            }
        }

        public void Update(float deltaTime)
        {
            if (scheduledEvents.Count == 0)
            {
                return;
            }

            if (isDirty)
            {
                scheduledEvents.Sort();
                isDirty = false;
            }

            for (int i = scheduledEvents.Count - 1; i >= 0; i--)
            {
                TimedEvent scheduledEvent = scheduledEvents[i];
                scheduledEvent.Update(deltaTime);

                if (scheduledEvent.IsExpired)
                {
                    scheduledEvent.Rescheduled -= OnRescheduled;
                    expiredEvents.Add(scheduledEvent);
                    scheduledEvents.RemoveAt(i);
                    scheduledEventsHash.Remove(scheduledEvent);
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
                int index = expiredEvents.Count - 1;
                expiredEvent = expiredEvents[index];
                expiredEvent.Expire();
                expiredEvents.RemoveAt(index);
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
                timedEvent.Rescheduled -= OnRescheduled;
            }

            scheduledEvents.Clear();
            scheduledEventsHash.Clear();
            expiredEvents.Clear();
            isDirty = false;
        }

        private void OnRescheduled(TimedEvent timedEvent)
        {
            isDirty = true;
        }
    }
}
