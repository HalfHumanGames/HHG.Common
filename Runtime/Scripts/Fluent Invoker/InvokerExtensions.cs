using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class InvokerExtensions
    {
        public static Invoker Invoker(this MonoBehaviour mono)
        {
            return new Invoker(mono);
        }

        public static ICanAddFrequencyOrTerminator After(this Invoker invoker, float seconds, bool realtime = false)
        {
            return invoker.After(seconds, realtime);
        }

        public static ICanAddFrequencyOrTerminator AfterRealtime(this Invoker invoker, float seconds)
        {
            return invoker.After(seconds, true);
        }

        public static ICanAddFrequencyOrTerminator NextFrame(this Invoker invoker)
        {
            return invoker.NextFrame();
        }

        public static ICanAddFrequencyOrTerminator NextFixed(this Invoker invoker)
        {
            return invoker.NextFixed();
        }

        public static ICanAddFrequencyOrTerminator When(this Invoker invoker, Func<bool> check)
        {
            return invoker.When(check);
        }

        public static ICanAddTerminator Every(this Invoker invoker, float seconds)
        {
            return invoker.Every(seconds);
        }

        public static ICanAddTerminator EachFrame(this Invoker invoker)
        {
            return invoker.EachFrame();
        }

        public static ICanAddTerminator EachFixed(this Invoker invoker)
        {
            return invoker.EachFixed();
        }

        public static ICanDo Forever(this Invoker invoker)
        {
            return invoker.Forever();
        }

        public static ICanDo Repeat(this Invoker invoker, int repetitions)
        {
            return invoker.Repeat(repetitions);
        }

        public static ICanDo While(this Invoker invoker, Func<bool> check)
        {
            return invoker.While(check);
        }

        public static ICanDo Until(this Invoker invoker, Func<bool> check)
        {
            return invoker.Until(check);
        }

        public static Coroutine After(this Invoker invoker, float seconds, Action<object[]> func)
        {
            return invoker.After(seconds).Do(func);
        }

        public static Coroutine After(this Invoker invoker, float seconds, bool realtime, Action<object[]> func)
        {
            return invoker.After(seconds, realtime).Do(func);
        }

        public static Coroutine AfterRealtime(this Invoker invoker, float seconds, Action<object[]> func)
        {
            return invoker.After(seconds, true).Do(func);
        }

        public static Coroutine NextFrame(this Invoker invoker, Action<object[]> func)
        {
            return invoker.NextFrame().Do(func);
        }

        public static Coroutine NextFixed(this Invoker invoker, Action<object[]> func)
        {
            return invoker.NextFixed().Do(func);
        }

        public static Coroutine When(this Invoker invoker, Func<bool> check, Action<object[]> func)
        {
            return invoker.When(check).Do(func);
        }

        public static Coroutine Every(this Invoker invoker, float seconds, Action<object[]> func)
        {
            return invoker.Every(seconds).Do(func);
        }

        public static Coroutine EachFrame(this Invoker invoker, Action<object[]> func)
        {
            return invoker.EachFrame().Do(func);
        }

        public static Coroutine EachFixed(this Invoker invoker, Action<object[]> func)
        {
            return invoker.EachFixed().Do(func);
        }

        public static Coroutine Forever(this Invoker invoker, Action<object[]> func)
        {
            return invoker.Forever().Do(func);
        }

        public static Coroutine Repeat(this Invoker invoker, int repetitions, Action<object[]> func)
        {
            return invoker.Repeat(repetitions).Do(func);
        }

        public static Coroutine While(this Invoker invoker, Func<bool> check, Action<object[]> func)
        {
            return invoker.While(check).Do(func);
        }

        public static Coroutine Until(this Invoker invoker, Func<bool> check, Action<object[]> func)
        {
            return invoker.Until(check).Do(func);
        }
    }
}