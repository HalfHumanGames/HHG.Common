using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
	public interface ICanDo
	{
		Coroutine Do(Action<object[]> action, params object[] args);
	} 
}