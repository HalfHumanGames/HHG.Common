using System;
using System.Collections.Generic;
using System.Linq;

namespace HHG.Common.Runtime
{
    public abstract class TimedEvent : IComparable<TimedEvent>
    {
        public object WeakContext => weakContext;
        public bool IsExpired => timeRemaining <= 0f;
        public float TimeRemaining => timeRemaining;

        protected object weakContext;
        protected float timeRemaining;

        public event Action<TimedEvent, float> WeakSchedule;
        public event Action<TimedEvent, float> WeakRescheduled;
        public event Action<TimedEvent, float> WeakUpdate;
        public event Action<TimedEvent> WeakUnscheduled;
        public event Action<TimedEvent> WeakExpired;

        public TimedEvent()
        {

        }

        public TimedEvent(float timeRemaining = 0f, object ctx = default)
        {
            Initialize(timeRemaining, ctx);
        }

        public virtual void Initialize(float duration = 0f, object ctx = default)
        {
            timeRemaining = duration;
            weakContext = ctx;
            WeakSchedule = null;
            WeakRescheduled = null;
            WeakUpdate = null;
            WeakUnscheduled = null;
            WeakExpired = null;
        }

        public void Schedule(float duration)
        {
            OnSchedule(duration);
        }

        public void Update(float timeElapsed)
        {
            timeRemaining -= timeElapsed;

            OnUpdate(timeElapsed);

            if (timeRemaining < 0f)
            {
                timeRemaining = 0f;
            }
        }

        public void Reschedule(float duration)
        {
            if (timeRemaining != duration)
            {
                timeRemaining = duration;
                OnReschedule(duration);
            }
        }

        public void Unschedule()
        {
            OnUnschedule();
        }

        public void Expire()
        {
            OnExpire();   
        }

        public int CompareTo(TimedEvent other)
        {
            return timeRemaining.CompareTo(other.timeRemaining);
        }

        override public string ToString()
        {
            return $"TimedEvent: {timeRemaining:0.00}s - {weakContext}";
        }

        protected virtual void OnSchedule(float duration)
        {
            WeakSchedule?.Invoke(this, duration);

            if (weakContext is ITimedEventListener listener)
            {
                listener.OnTimedEventScheduled(this, duration);
            }
        }

        protected virtual void OnReschedule(float duration)
        {
            WeakRescheduled?.Invoke(this, duration);

            if (weakContext is ITimedEventListener listener)
            {
                listener.OnTimedEventRescheduled(this, duration);
            }
        }

        protected virtual void OnUpdate(float timeElapsed)
        {
            WeakUpdate?.Invoke(this, timeElapsed);

            if (weakContext is ITimedEventListener listener)
            {
                listener.OnTimedEventUpdated(this, timeElapsed);
            }
        }

        protected virtual void OnUnschedule()
        {
            WeakUnscheduled?.Invoke(this);

            if (weakContext is ITimedEventListener listener)
            {
                listener.OnTimedEventUnscheduled(this);
            }
        }

        protected virtual void OnExpire()
        {
            WeakExpired?.Invoke(this);

            if (weakContext is ITimedEventListener listener)
            {
                listener.OnTimedEventExpired(this);
            }
        }
    }

    public class TimedEvent<T> : TimedEvent
    {
        public T Context => context;

        private T context;

        public event Action<TimedEvent<T>, float> Scheduled;
        public event Action<TimedEvent<T>, float> Rescheduled;
        public event Action<TimedEvent<T>, float> Updated;
        public event Action<TimedEvent<T>> Unscheduled;
        public event Action<TimedEvent<T>> Expired;

        public TimedEvent()
        {

        }

        public TimedEvent(float timeRemaining = 0f, T ctx = default)
        {
            Initialize(timeRemaining, ctx);
        }

        public void Initialize(float duration = 0f, T ctx = default)
        {
            context = ctx;
            Scheduled = null;
            Rescheduled = null;
            Updated = null;
            Unscheduled = null;
            Expired = null;
            base.Initialize(duration, ctx);
        }

        public override void Initialize(float duration = 0, object ctx = null)
        {
            if (ctx == null)
            {
                Initialize(duration, default);
            }
            else if (ctx is T typedCtx)
            {
                Initialize(duration, typedCtx);
            }
            else
            {
                throw new ArgumentException($"Invalid context type: {ctx?.GetType()}");
            }
        }

        protected override void OnSchedule(float duration)
        {
            base.OnSchedule(duration);
            Scheduled?.Invoke(this, duration);

            if (context is ITimedEventListener<T> listener)
            {
                listener.OnTimedEventScheduled(this, duration);
            }
        }

        protected override void OnReschedule(float duration)
        {
            base.OnReschedule(duration);
            Rescheduled?.Invoke(this, duration);

            if (context is ITimedEventListener<T> listener)
            {
                listener.OnTimedEventRescheduled(this, duration);
            }
        }

        protected override void OnUpdate(float timeElapsed)
        {
            base.OnUpdate(timeElapsed);
            Updated?.Invoke(this, timeElapsed);

            if (context is ITimedEventListener<T> listener)
            {
                listener.OnTimedEventUpdated(this, timeElapsed);
            }
        }

        protected override void OnUnschedule()
        {
            base.OnUnschedule();
            Unscheduled?.Invoke(this);

            if (context is ITimedEventListener<T> listener)
            {
                listener.OnTimedEventUnscheduled(this);
            }
        }

        protected override void OnExpire()
        {
            base.OnExpire();
            Expired?.Invoke(this);

            if (context is ITimedEventListener<T> listener)
            {
                listener.OnTimedEventExpired(this);
            }
        }
    }

    public static class TimedEventExtensions
    {
        public static IEnumerable<TimedEvent<T>> ByContextType<T>(this IEnumerable<TimedEvent> enumerable)
        {
            return enumerable.OfType<TimedEvent<T>>();
        }

        public static IEnumerable<T> Contexts<T>(this IEnumerable<TimedEvent<T>> enumerable)
        {
            return enumerable.Select(x => x.Context);
        }

        public static TimedEvent<T> Find<T>(this IEnumerable<TimedEvent> enumerable, T context)
        {
            return enumerable.ByContextType<T>().FirstOrDefault(x => x.Context.Equals(context));
        }
    }
}