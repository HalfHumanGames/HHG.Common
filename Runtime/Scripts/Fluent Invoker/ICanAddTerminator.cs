using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public interface ICanAddTerminator : ICanDo
    {
        ICanDo Forever();
        ICanDo Repeat(int repetitions);
        ICanDo While(Func<bool> check);
        ICanDo Until(Func<bool> check);

        Coroutine Forever(Action<object[]> func);
        Coroutine Repeat(int repetitions, Action<object[]> func);
        Coroutine While(Func<bool> check, Action<object[]> func);
        Coroutine Until(Func<bool> check, Action<object[]> func);
    } 
}