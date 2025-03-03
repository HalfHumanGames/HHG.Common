namespace HHG.Common.Runtime
{
    public interface IBindable
	{
		T GetValue<T>(string name);
		void SetValue<T>(string name, T value);
		event System.Action stateUpdated;
	}

	public static class IBindableExtensions
	{
        public static object GetValue(this IBindable bindable, string name)
        {
            return bindable.GetValue<object>(name);
        }

        public static bool TryGetValue(this IBindable bindable, string name, out object value)
        {
            return (value = bindable.GetValue(name)) != null;
        }

        public static bool TryGetValue<T>(this IBindable bindable, string name, out T value)
		{
			return (value = bindable.GetValue<T>(name)) != null;
		}
	}
}