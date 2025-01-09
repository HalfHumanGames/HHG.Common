using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public interface ICanAddTerminator<T> : ICanDo<T>
    {
        ICanDo<T> RepeatForever();
        ICanDo<T> Repeat(int repetitions);
        ICanDo<T> While(Func<bool> check);
        ICanDo<T> Until(Func<bool> check);

        Coroutine RepeatForever(Action<T> func, T arg = default);
        Coroutine Repeat(int repetitions, Action<T> func, T arg = default);
        Coroutine While(Func<bool> check, Action<T> func, T arg = default);
        Coroutine Until(Func<bool> check, Action<T> func, T arg = default);
    } 
}