using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public interface ICanAddFrequencyOrTerminator<T> : ICanAddTerminator<T>
    {
        ICanAddTerminator<T> Every(float seconds);
        ICanAddTerminator<T> Every(float seconds, bool realtime = false);
        ICanAddTerminator<T> EveryRealtime(float seconds);
        ICanAddTerminator<T> EachFrame();
        ICanAddTerminator<T> EachFixed();

        Coroutine Every(float seconds, Action<T> func, T arg = default);
        Coroutine Every(float seconds, bool realtime, Action<T> func, T arg = default);
        Coroutine EveryRealtime(float seconds, Action<T> func, T arg = default);
        Coroutine EachFrame(Action<T> func, T arg = default);
        Coroutine EachFixed(Action<T> func, T arg = default);
    } 
}