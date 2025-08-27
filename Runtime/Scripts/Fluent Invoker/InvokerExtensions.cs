using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class InvokerExtensions
    {
        #region Invoker Extensions

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

        public static ICanAddTerminator<T> Every<T>(this Invoker<T> invoker, float seconds, bool realtime = false)
        {
            return invoker.Every(seconds, realtime);
        }

        public static ICanAddTerminator<T> EveryRealtime<T>(this Invoker<T> invoker, float seconds)
        {
            return invoker.Every(seconds, true);
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

        public static Coroutine Every<T>(this Invoker<T> invoker, float seconds, bool realtime, Action<T> func, T arg = default)
        {
            return invoker.Every(seconds, realtime).Do(func, arg);
        }

        public static Coroutine EveryRealtime<T>(this Invoker<T> invoker, float seconds, Action<T> func, T arg = default)
        {
            return invoker.Every(seconds, true).Do(func, arg);
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

        #endregion

        #region MonoBehaviour Generic Extensions

        public static ICanAddFrequencyOrTerminator<T> After<T>(this MonoBehaviour mono, float seconds, bool realtime = false)
        {
            return mono.GetInvokerFromObjectPool<T>().After(seconds, realtime);
        }

        public static ICanAddFrequencyOrTerminator<T> AfterRealtime<T>(this MonoBehaviour mono, float seconds)
        {
            return mono.GetInvokerFromObjectPool<T>().After(seconds, true);
        }

        public static ICanAddFrequencyOrTerminator<T> NextFrame<T>(this MonoBehaviour mono)
        {
            return mono.GetInvokerFromObjectPool<T>().NextFrame();
        }

        public static ICanAddFrequencyOrTerminator<T> NextFixed<T>(this MonoBehaviour mono)
        {
            return mono.GetInvokerFromObjectPool<T>().NextFixed();
        }

        public static ICanAddFrequencyOrTerminator<T> When<T>(this MonoBehaviour mono, Func<bool> check)
        {
            return mono.GetInvokerFromObjectPool<T>().When(check);
        }

        public static ICanAddTerminator<T> Every<T>(this MonoBehaviour mono, float seconds, bool realtime = false)
        {
            return mono.GetInvokerFromObjectPool<T>().Every(seconds, realtime);
        }

        public static ICanAddTerminator<T> EveryRealtime<T>(this MonoBehaviour mono, float seconds)
        {
            return mono.GetInvokerFromObjectPool<T>().Every(seconds, true);
        }

        public static ICanAddTerminator<T> EachFrame<T>(this MonoBehaviour mono)
        {
            return mono.GetInvokerFromObjectPool<T>().EachFrame();
        }

        public static ICanAddTerminator<T> EachFixed<T>(this MonoBehaviour mono)
        {
            return mono.GetInvokerFromObjectPool<T>().EachFixed();
        }

        public static ICanDo<T> RepeatForever<T>(this MonoBehaviour mono)
        {
            return mono.GetInvokerFromObjectPool<T>().RepeatForever();
        }

        public static ICanDo<T> Repeat<T>(this MonoBehaviour mono, int repetitions)
        {
            return mono.GetInvokerFromObjectPool<T>().Repeat(repetitions);
        }

        public static ICanDo<T> While<T>(this MonoBehaviour mono, Func<bool> check)
        {
            return mono.GetInvokerFromObjectPool<T>().While(check);
        }

        public static ICanDo<T> Until<T>(this MonoBehaviour mono, Func<bool> check)
        {
            return mono.GetInvokerFromObjectPool<T>().Until(check);
        }

        public static Coroutine After<T>(this MonoBehaviour mono, float seconds, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().After(seconds).Do(func, arg);
        }

        public static Coroutine After<T>(this MonoBehaviour mono, float seconds, bool realtime, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().After(seconds, realtime).Do(func, arg);
        }

        public static Coroutine AfterRealtime<T>(this MonoBehaviour mono, float seconds, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().After(seconds, true).Do(func, arg);
        }

        public static Coroutine NextFrame<T>(this MonoBehaviour mono, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().NextFrame().Do(func, arg);
        }

        public static Coroutine NextFixed<T>(this MonoBehaviour mono, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().NextFixed().Do(func, arg);
        }

        public static Coroutine When<T>(this MonoBehaviour mono, Func<bool> check, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().When(check).Do(func, arg);
        }

        public static Coroutine Every<T>(this MonoBehaviour mono, float seconds, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().Every(seconds).Do(func, arg);
        }

        public static Coroutine Every<T>(this MonoBehaviour mono, float seconds, bool realtime, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().Every(seconds, realtime).Do(func, arg);
        }

        public static Coroutine EveryRealtime<T>(this MonoBehaviour mono, float seconds, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().Every(seconds, true).Do(func, arg);
        }

        public static Coroutine EachFrame<T>(this MonoBehaviour mono, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().EachFrame().Do(func, arg);
        }

        public static Coroutine EachFixed<T>(this MonoBehaviour mono, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().EachFixed().Do(func, arg);
        }

        public static Coroutine RepeatForever<T>(this MonoBehaviour mono, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().RepeatForever().Do(func, arg);
        }

        public static Coroutine Repeat<T>(this MonoBehaviour mono, int repetitions, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().Repeat(repetitions).Do(func, arg);
        }

        public static Coroutine While<T>(this MonoBehaviour mono, Func<bool> check, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().While(check).Do(func, arg);
        }

        public static Coroutine Until<T>(this MonoBehaviour mono, Func<bool> check, Action<T> func, T arg = default)
        {
            return mono.GetInvokerFromObjectPool<T>().Until(check).Do(func, arg);
        }

        #endregion

        #region MonoBehaviour Untyped Extensions

        public static ICanAddFrequencyOrTerminator<object> After(this MonoBehaviour mono, float seconds, bool realtime = false)
        {
            return mono.GetInvokerFromObjectPool().After(seconds, realtime);
        }

        public static ICanAddFrequencyOrTerminator<object> AfterRealtime(this MonoBehaviour mono, float seconds)
        {
            return mono.GetInvokerFromObjectPool().After(seconds, true);
        }

        public static ICanAddFrequencyOrTerminator<object> NextFrame(this MonoBehaviour mono)
        {
            return mono.GetInvokerFromObjectPool().NextFrame();
        }

        public static ICanAddFrequencyOrTerminator<object> NextFixed(this MonoBehaviour mono)
        {
            return mono.GetInvokerFromObjectPool().NextFixed();
        }

        public static ICanAddFrequencyOrTerminator<object> When(this MonoBehaviour mono, Func<bool> check)
        {
            return mono.GetInvokerFromObjectPool().When(check);
        }

        public static ICanAddTerminator<object> Every(this MonoBehaviour mono, float seconds, bool realtime = false)
        {
            return mono.GetInvokerFromObjectPool().Every(seconds, realtime);
        }

        public static ICanAddTerminator<object> EveryRealtime(this MonoBehaviour mono, float seconds)
        {
            return mono.GetInvokerFromObjectPool().Every(seconds, true);
        }

        public static ICanAddTerminator<object> EachFrame(this MonoBehaviour mono)
        {
            return mono.GetInvokerFromObjectPool().EachFrame();
        }

        public static ICanAddTerminator<object> EachFixed(this MonoBehaviour mono)
        {
            return mono.GetInvokerFromObjectPool().EachFixed();
        }

        public static ICanDo<object> RepeatForever(this MonoBehaviour mono)
        {
            return mono.GetInvokerFromObjectPool().RepeatForever();
        }

        public static ICanDo<object> Repeat(this MonoBehaviour mono, int repetitions)
        {
            return mono.GetInvokerFromObjectPool().Repeat(repetitions);
        }

        public static ICanDo<object> While(this MonoBehaviour mono, Func<bool> check)
        {
            return mono.GetInvokerFromObjectPool().While(check);
        }

        public static ICanDo<object> Until(this MonoBehaviour mono, Func<bool> check)
        {
            return mono.GetInvokerFromObjectPool().Until(check);
        }

        public static Coroutine After(this MonoBehaviour mono, float seconds, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().After(seconds).Do(func, arg);
        }

        public static Coroutine After(this MonoBehaviour mono, float seconds, bool realtime, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().After(seconds, realtime).Do(func, arg);
        }

        public static Coroutine AfterRealtime(this MonoBehaviour mono, float seconds, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().After(seconds, true).Do(func, arg);
        }

        public static Coroutine NextFrame(this MonoBehaviour mono, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().NextFrame().Do(func, arg);
        }

        public static Coroutine NextFixed(this MonoBehaviour mono, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().NextFixed().Do(func, arg);
        }

        public static Coroutine When(this MonoBehaviour mono, Func<bool> check, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().When(check).Do(func, arg);
        }

        public static Coroutine Every(this MonoBehaviour mono, float seconds, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().Every(seconds).Do(func, arg);
        }

        public static Coroutine Every(this MonoBehaviour mono, float seconds, bool realtime, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().Every(seconds, realtime).Do(func, arg);
        }

        public static Coroutine EveryRealtime(this MonoBehaviour mono, float seconds, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().Every(seconds, true).Do(func, arg);
        }

        public static Coroutine EachFrame(this MonoBehaviour mono, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().EachFrame().Do(func, arg);
        }

        public static Coroutine EachFixed(this MonoBehaviour mono, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().EachFixed().Do(func, arg);
        }

        public static Coroutine RepeatForever(this MonoBehaviour mono, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().RepeatForever().Do(func, arg);
        }

        public static Coroutine Repeat(this MonoBehaviour mono, int repetitions, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().Repeat(repetitions).Do(func, arg);
        }

        public static Coroutine While(this MonoBehaviour mono, Func<bool> check, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().While(check).Do(func, arg);
        }

        public static Coroutine Until(this MonoBehaviour mono, Func<bool> check, Action<object> func, object arg = default)
        {
            return mono.GetInvokerFromObjectPool().Until(check).Do(func, arg);
        }

        #endregion

        #region Helper Methods

        // ObjectPool.Release will fail if call StopCoroutine on the invoker
        private static Invoker GetInvokerFromObjectPool(this MonoBehaviour mono)
        {
            Invoker invoker = Pool.Get<Invoker>();
            invoker.Initialize(mono, () => Pool.Release(invoker));
            return invoker;
        }

        // ObjectPool.Release will fail if call StopCoroutine on the invoker
        private static Invoker<T> GetInvokerFromObjectPool<T>(this MonoBehaviour mono)
        {
            Invoker<T> invoker = Pool.Get<Invoker<T>>();
            invoker.Initialize(mono, () => Pool.Release(invoker));
            return invoker;
        }

        #endregion
    }
}