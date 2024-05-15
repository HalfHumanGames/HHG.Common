using System;

namespace HHG.Common.Runtime
{
    public interface IBindable
	{
		bool TryGetValue<T>(string name, out T value);
		bool TrySetValue<T>(string name, T value);
		event Action Updated;
	}

	public static class IBindableExtensions
	{
        public static T GetValue<T>(this IBindable bindable, string name) => bindable.TryGetValue(name, out T value) ? value : default;
		public static void SetValue<T>(this IBindable bindable, string name, T value) => bindable.SetValue(name, value);
    }
}