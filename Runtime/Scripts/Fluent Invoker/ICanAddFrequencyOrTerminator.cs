using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public interface ICanAddFrequencyOrTerminator : ICanAddTerminator
    {
        ICanAddTerminator Every(float seconds);
        ICanAddTerminator Every(float seconds, bool realtime = false);
        ICanAddTerminator EveryRealtime(float seconds);
        ICanAddTerminator EachFrame();
        ICanAddTerminator EachFixed();

        Coroutine Every(float seconds, Action<object[]> func);
        Coroutine Every(float seconds, bool realtime, Action<object[]> func);
        Coroutine EveryRealtime(float seconds, Action<object[]> func);
        Coroutine EachFrame(Action<object[]> func);
        Coroutine EachFixed(Action<object[]> func);
    } 
}