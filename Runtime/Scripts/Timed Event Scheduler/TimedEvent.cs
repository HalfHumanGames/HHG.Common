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

        public event Action<TimedEvent> WeakExpired;
        public event Action<TimedEvent> WeakRescheduled;

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
            WeakExpired = null;
            WeakRescheduled = null;
        }

        public void Update(float timeElapsed)
        {
            timeRemaining -= timeElapsed;

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
                OnReschedule();
            }
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

        protected virtual void OnReschedule()
        {
            WeakRescheduled?.Invoke(this);
        }

        protected virtual void OnExpire()
        {
            WeakExpired?.Invoke(this);
        }
    }

    public class TimedEvent<T> : TimedEvent
    {
        public T Context => context;

        private T context;

        public event Action<TimedEvent<T>> Expired;
        public event Action<TimedEvent<T>> Rescheduled;

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
            Expired = null;
            Rescheduled = null;
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

        protected override void OnReschedule()
        {
            base.OnReschedule();
            Rescheduled?.Invoke(this);
        }

        protected override void OnExpire()
        {
            base.OnExpire();
            Expired?.Invoke(this);
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
    }
}