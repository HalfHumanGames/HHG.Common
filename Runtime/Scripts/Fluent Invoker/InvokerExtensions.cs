using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class InvokerExtensions
    {
        public static Invoker Invoker(this MonoBehaviour mono) 
        {
            Invoker invoker = ObjectPool.Get<Invoker>();
            invoker.Initialize(mono);
            invoker.Done += () =>
            {
                invoker.Reset();
                ObjectPool.Release(invoker);
            };
            return invoker;
        }

        public static Invoker<T> Invoker<T>(this MonoBehaviour mono)
        {
            Invoker<T> invoker = ObjectPool.Get<Invoker<T>>();
            invoker.Initialize(mono);
            invoker.Done += () =>
            {
                invoker.Reset();
                ObjectPool.Release(invoker);
            };
            return invoker;
        }

        public static ICanAddFrequencyOrTerminator<T> After<T>(this Invoker<T> invoker, float seconds, bool realtime = false)
        {
            return invoker.After(seconds, realtime);
        }

        public static ICanAddFrequencyOrTerminator<T> AfterRealtime<T>(this Invoker<T> invoker, float seconds)
        {
            return invoker.After(seconds, true);
        }

        public static ICanAddFrequencyOrTerminator<T> NextFrame<T>(this Invoker<T> invoker)
        {
            return invoker.NextFrame();
        }

        public static ICanAddFrequencyOrTerminator<T> NextFixed<T>(this Invoker<T> invoker)
        {
            return invoker.NextFixed();
        }

        public static ICanAddFrequencyOrTerminator<T> When<T>(this Invoker<T> invoker, Func<bool> check)
        {
            return invoker.When(check);
        }

        public static ICanAddTerminator<T> Every<T>(this Invoker<T> invoker, float seconds)
        {
            return invoker.Every(seconds);
        }

        public static ICanAddTerminator<T> EachFrame<T>(this Invoker<T> invoker)
        {
            return invoker.EachFrame();
        }

        public static ICanAddTerminator<T> EachFixed<T>(this Invoker<T> invoker)
        {
            return invoker.EachFixed();
        }

        public static ICanDo<T> RepeatForever<T>(this Invoker<T> invoker)
        {
            return invoker.RepeatForever();
        }

        public static ICanDo<T> Repeat<T>(this Invoker<T> invoker, int repetitions)
        {
            return invoker.Repeat(repetitions);
        }

        public static ICanDo<T> While<T>(this Invoker<T> invoker, Func<bool> check)
        {
            return invoker.While(check);
        }

        public static ICanDo<T> Until<T>(this Invoker<T> invoker, Func<bool> check)
        {
            return invoker.Until(check);
        }

        public static Coroutine After<T>(this Invoker<T> invoker, float seconds, Action<T> func, T arg = default)
        {
            return invoker.After(seconds).Do(func, arg);
        }

        public static Coroutine After<T>(this Invoker<T> invoker, float seconds, bool realtime, Action<T> func, T arg = default)
        {
            return invoker.After(seconds, realtime).Do(func, arg);
        }

        public static Coroutine AfterRealtime<T>(this Invoker<T> invoker, float seconds, Action<T> func, T arg = default)
        {
            return invoker.After(seconds, true).Do(func, arg);
        }

        public static Coroutine NextFrame<T>(this Invoker<T> invoker, Action<T> func, T arg = default)
        {
            return invoker.NextFrame().Do(func, arg);
        }

        public static Coroutine NextFixed<T>(this Invoker<T> invoker, Action<T> func, T arg = default)
        {
            return invoker.NextFixed().Do(func, arg);
        }

        public static Coroutine When<T>(this Invoker<T> invoker, Func<bool> check, Action<T> func, T arg = default)
        {
            return invoker.When(check).Do(func, arg);
        }

        public static Coroutine Every<T>(this Invoker<T> invoker, float seconds, Action<T> func, T arg = default)
        {
            return invoker.Every(seconds).Do(func, arg);
        }

        public static Coroutine EachFrame<T>(this Invoker<T> invoker, Action<T> func, T arg = default)
        {
            return invoker.EachFrame().Do(func, arg);
        }

        public static Coroutine EachFixed<T>(this Invoker<T> invoker, Action<T> func, T arg = default)
        {
            return invoker.EachFixed().Do(func, arg);
        }

        public static Coroutine RepeatForever<T>(this Invoker<T> invoker, Action<T> func, T arg = default)
        {
            return invoker.RepeatForever().Do(func, arg);
        }

        public static Coroutine Repeat<T>(this Invoker<T> invoker, int repetitions, Action<T> func, T arg = default)
        {
            return invoker.Repeat(repetitions).Do(func, arg);
        }

        public static Coroutine While<T>(this Invoker<T> invoker, Func<bool> check, Action<T> func, T arg = default)
        {
            return invoker.While(check).Do(func, arg);
        }

        public static Coroutine Until<T>(this Invoker<T> invoker, Func<bool> check, Action<T> func, T arg = default)
        {
            return invoker.Until(check).Do(func, arg);
        }
    }
}