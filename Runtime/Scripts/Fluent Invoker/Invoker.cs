using System;
using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class Invoker : Invoker<object>
    {
        public Invoker()
        {

        }

        public Invoker(MonoBehaviour mono)
        {
            invoker = mono;
        }
    }

    public class Invoker<T> : ICanAddFrequencyOrTerminator<T>, ICanAddTerminator<T>
    {
        protected float startDelay;
        protected InvokeDelayer startDelayer;
        protected float delay;
        protected bool realtime;
        protected int repetitions = 1;
        protected Func<bool> when;
        protected Func<bool> check;
        protected InvokeFrequency frequency;
        protected InvokeTerminator terminator;
        protected MonoBehaviour invoker;

        protected enum InvokeDelayer { None, Seconds, SecondsRealtime, Frame, Fixed, When }
        protected enum InvokeFrequency { Every, EachFrame, EachFixed }
        protected enum InvokeTerminator { Once, Repeat, RepeatForever, While, Until }

        public event Action Done;

        public Invoker()
        {

        }

        public Invoker(MonoBehaviour mono)
        {
            invoker = mono;
        }

        public void Initialize(MonoBehaviour mono)
        {
            Reset();
            invoker = mono;
        }

        public void Reset()
        {
            startDelay = 0f;
            startDelayer = InvokeDelayer.None;
            delay = 0f;
            realtime = false;
            repetitions = 1;
            when = null;
            check = null;
            frequency = InvokeFrequency.Every;
            terminator = InvokeTerminator.Once;
            invoker = null;
            Done = null;
        }

        public ICanAddFrequencyOrTerminator<T> After(float seconds, bool realtime = false)
        {
            startDelay = seconds;
            startDelayer = realtime ? InvokeDelayer.SecondsRealtime : InvokeDelayer.Seconds;
            return this;
        }

        public ICanAddFrequencyOrTerminator<T> AfterRealtime(float seconds)
        {
            startDelay = seconds;
            startDelayer = InvokeDelayer.SecondsRealtime;
            return this;
        }

        public ICanAddFrequencyOrTerminator<T> NextFrame()
        {
            startDelayer = InvokeDelayer.Frame;
            return this;
        }

        public ICanAddFrequencyOrTerminator<T> NextFixed()
        {
            startDelayer = InvokeDelayer.Fixed;
            return this;
        }

        public ICanAddFrequencyOrTerminator<T> When(Func<bool> func)
        {
            startDelayer = InvokeDelayer.When;
            when = func;
            return this;
        }

        public ICanAddTerminator<T> Every(float seconds)
        {
            frequency = InvokeFrequency.Every;
            delay = seconds;
            return this;
        }

        public ICanAddTerminator<T> Every(float seconds, bool real = false)
        {
            frequency = InvokeFrequency.Every;
            delay = seconds;
            realtime = real;
            return this;
        }

        public ICanAddTerminator<T> EveryRealtime(float seconds)
        {
            frequency = InvokeFrequency.Every;
            delay = seconds;
            realtime = true;
            return this;
        }

        public ICanAddTerminator<T> EachFixed()
        {
            frequency = InvokeFrequency.EachFixed;
            return this;
        }

        public ICanAddTerminator<T> EachFrame()
        {
            frequency = InvokeFrequency.EachFrame;
            return this;
        }

        public ICanDo<T> RepeatForever()
        {
            terminator = InvokeTerminator.RepeatForever;
            return this;
        }

        public ICanDo<T> Repeat(int num)
        {
            terminator = InvokeTerminator.Repeat;
            repetitions = num;
            return this;
        }

        public ICanDo<T> While(Func<bool> func)
        {
            terminator = InvokeTerminator.While;
            check = func;
            return this;
        }

        public ICanDo<T> Until(Func<bool> func)
        {
            terminator = InvokeTerminator.Until;
            check = func;
            return this;
        }

        public Coroutine Every(float seconds, Action<T> func, T arg = default)
        {
            terminator = InvokeTerminator.RepeatForever;
            frequency = InvokeFrequency.Every;
            delay = seconds;
            return Do(func, arg);
        }

        public Coroutine Every(float seconds, bool real, Action<T> func, T arg = default)
        {
            terminator = InvokeTerminator.RepeatForever;
            frequency = InvokeFrequency.Every;
            delay = seconds;
            realtime = real;
            return Do(func, arg);
        }

        public Coroutine EveryRealtime(float seconds, Action<T> func, T arg = default)
        {
            terminator = InvokeTerminator.RepeatForever;
            frequency = InvokeFrequency.Every;
            delay = seconds;
            realtime = true;
            return Do(func, arg);
        }

        public Coroutine EachFixed(Action<T> func, T arg = default)
        {
            terminator = InvokeTerminator.RepeatForever;
            frequency = InvokeFrequency.EachFixed;
            return Do(func, arg);
        }

        public Coroutine EachFrame(Action<T> func, T arg = default)
        {
            terminator = InvokeTerminator.RepeatForever;
            frequency = InvokeFrequency.EachFrame;
            return Do(func, arg);
        }

        public Coroutine Repeat(int num, Action<T> func, T arg = default)
        {
            terminator = InvokeTerminator.Repeat;
            repetitions = num;
            return Do(func, arg);
        }

        public Coroutine RepeatForever(Action<T> func, T arg = default)
        {
            terminator = InvokeTerminator.RepeatForever;
            return Do(func, arg);
        }

        public Coroutine While(Func<bool> func, Action<T> func2, T arg = default)
        {
            terminator = InvokeTerminator.While;
            check = func;
            return Do(func2, arg);
        }

        public Coroutine Until(Func<bool> func, Action<T> func2, T arg = default)
        {
            terminator = InvokeTerminator.Until;
            check = func;
            return Do(func2, arg);
        }

        public Coroutine Do(Action<T> action, T arg = default)
        {
            return invoker.StartCoroutine(invoke(action, arg));
        }

        private IEnumerator invoke(Action<T> action, T arg)
        {
            switch (startDelayer)
            {
                case InvokeDelayer.None:
                    break;
                case InvokeDelayer.Seconds:
                    if (startDelay > 0f) yield return new WaitForSeconds(startDelay);
                    break;
                case InvokeDelayer.SecondsRealtime:
                    if (startDelay > 0f) yield return new WaitForSecondsRealtime(startDelay);
                    break;
                case InvokeDelayer.Frame:
                    yield return new WaitForEndOfFrame();
                    break;
                case InvokeDelayer.Fixed:
                    yield return new WaitForFixedUpdate();
                    break;
                case InvokeDelayer.When:
                    if (!when()) yield return new WaitUntil(when);
                    break;
            }

            do
            {
                switch (frequency)
                {
                    case InvokeFrequency.Every:
                        action(arg);
                        yield return realtime ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay);
                        break;
                    case InvokeFrequency.EachFrame:
                        action(arg);
                        yield return new WaitForEndOfFrame();
                        break;
                    case InvokeFrequency.EachFixed:
                        action(arg);
                        yield return new WaitForFixedUpdate();
                        break;
                }

                repetitions--;

            } while (CanContinue());

            Done?.Invoke();
        }

        private bool CanContinue()
        {
            switch (terminator)
            {
                case InvokeTerminator.Repeat:
                    return repetitions == -1f || repetitions > 0;
                case InvokeTerminator.RepeatForever:
                    return true;
                case InvokeTerminator.Until:
                    return !check();
                case InvokeTerminator.While:
                    return check();
                case InvokeTerminator.Once:
                    return false;
            }
            return false; // Should never get here
        }
    } 
}
