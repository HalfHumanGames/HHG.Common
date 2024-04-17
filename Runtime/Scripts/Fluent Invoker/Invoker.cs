using System;
using System.Collections;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class Invoker : ICanAddFrequencyOrTerminator, ICanAddTerminator
    {
        private float startDelay;
        private InvokeDelayer startDelayer;
        private float delay;
        private bool realtime;
        private int repetitions = 1;
        private Func<bool> when;
        private Func<bool> check;
        private InvokeFrequency frequency;
        private InvokeTerminator terminator;
        private MonoBehaviour invoker;

        private enum InvokeDelayer { None, Seconds, SecondsRealtime, Frame, Fixed, When }
        private enum InvokeFrequency { Every, EachFrame, EachFixed }
        private enum InvokeTerminator { Once, Repeat, While, Until }

        public Invoker(MonoBehaviour mono)
        {
            invoker = mono;
        }

        public ICanAddFrequencyOrTerminator After(float seconds, bool realtime = false)
        {
            startDelay = seconds;
            startDelayer = realtime ? InvokeDelayer.SecondsRealtime : InvokeDelayer.Seconds;
            return this;
        }

        public ICanAddFrequencyOrTerminator AfterRealtime(float seconds)
        {
            startDelay = seconds;
            startDelayer = InvokeDelayer.SecondsRealtime;
            return this;
        }

        public ICanAddFrequencyOrTerminator NextFrame()
        {
            startDelayer = InvokeDelayer.Frame;
            return this;
        }

        public ICanAddFrequencyOrTerminator NextFixed()
        {
            startDelayer = InvokeDelayer.Fixed;
            return this;
        }

        public ICanAddFrequencyOrTerminator When(Func<bool> func)
        {
            startDelayer = InvokeDelayer.When;
            when = func;
            return this;
        }

        public ICanAddTerminator Every(float seconds)
        {
            frequency = InvokeFrequency.Every;
            delay = seconds;
            return this;
        }

        public ICanAddTerminator Every(float seconds, bool real = false)
        {
            frequency = InvokeFrequency.Every;
            delay = seconds;
            realtime = real;
            return this;
        }

        public ICanAddTerminator EveryRealtime(float seconds)
        {
            frequency = InvokeFrequency.Every;
            delay = seconds;
            realtime = true;
            return this;
        }

        public ICanAddTerminator EachFixed()
        {
            frequency = InvokeFrequency.EachFixed;
            return this;
        }

        public ICanAddTerminator EachFrame()
        {
            frequency = InvokeFrequency.EachFrame;
            return this;
        }

        public ICanDo Forever()
        {
            terminator = InvokeTerminator.Repeat;
            repetitions = -1;
            return this;
        }

        public ICanDo Repeat(int num)
        {
            terminator = InvokeTerminator.Repeat;
            repetitions = num;
            return this;
        }

        public ICanDo While(Func<bool> func)
        {
            terminator = InvokeTerminator.While;
            check = func;
            return this;
        }

        public ICanDo Until(Func<bool> func)
        {
            terminator = InvokeTerminator.Until;
            check = func;
            return this;
        }

        public Coroutine Every(float seconds, Action<object[]> func)
        {
            frequency = InvokeFrequency.Every;
            delay = seconds;
            return Do(func);
        }

        public Coroutine Every(float seconds, bool real, Action<object[]> func)
        {
            frequency = InvokeFrequency.Every;
            delay = seconds;
            realtime = real;
            return Do(func);
        }

        public Coroutine EveryRealtime(float seconds, Action<object[]> func)
        {
            frequency = InvokeFrequency.Every;
            delay = seconds;
            realtime = true;
            return Do(func);
        }

        public Coroutine EachFixed(Action<object[]> func)
        {
            frequency = InvokeFrequency.EachFixed;
            return Do(func);
        }

        public Coroutine EachFrame(Action<object[]> func)
        {
            frequency = InvokeFrequency.EachFrame;
            return Do(func);
        }

        public Coroutine Forever(Action<object[]> func)
        {
            terminator = InvokeTerminator.Repeat;
            repetitions = -1;
            return Do(func);
        }

        public Coroutine Repeat(int num, Action<object[]> func)
        {
            terminator = InvokeTerminator.Repeat;
            repetitions = num;
            return Do(func);
        }

        public Coroutine While(Func<bool> func, Action<object[]> func2)
        {
            terminator = InvokeTerminator.While;
            check = func;
            return Do(func2);
        }

        public Coroutine Until(Func<bool> func, Action<object[]> func2)
        {
            terminator = InvokeTerminator.Until;
            check = func;
            return Do(func2);
        }

        public Coroutine Do(Action<object[]> action, params object[] args)
        {
            return invoker.StartCoroutine(invoke(action, args));
        }

        private IEnumerator invoke(Action<object[]> action, object[] args)
        {
            switch (startDelayer)
            {
                case InvokeDelayer.None:
                    break;
                case InvokeDelayer.Seconds:
                    yield return new WaitForSeconds(startDelay);
                    break;
                case InvokeDelayer.SecondsRealtime:
                    yield return new WaitForSecondsRealtime(startDelay);
                    break;
                case InvokeDelayer.Frame:
                    yield return new WaitForEndOfFrame();
                    break;
                case InvokeDelayer.Fixed:
                    yield return new WaitForFixedUpdate();
                    break;
                case InvokeDelayer.When:
                    yield return new WaitUntil(when);
                    break;
            }

            do
            {
                switch (frequency)
                {
                    case InvokeFrequency.Every:
                        action(args);
                        yield return realtime ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay);
                        break;
                    case InvokeFrequency.EachFrame:
                        action(args);
                        yield return new WaitForEndOfFrame();
                        break;
                    case InvokeFrequency.EachFixed:
                        action(args);
                        yield return new WaitForFixedUpdate();
                        break;
                }

                repetitions--;

            } while (CanContinue());
        }

        private bool CanContinue()
        {
            switch (terminator)
            {
                case InvokeTerminator.Repeat:
                    return repetitions == -1f || repetitions > 0;
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
