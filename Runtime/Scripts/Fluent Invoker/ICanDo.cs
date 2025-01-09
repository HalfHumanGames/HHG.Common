using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
	public interface ICanDo<T>
    {
		Coroutine Do(Action<T> action, T arg = default);
	} 
}