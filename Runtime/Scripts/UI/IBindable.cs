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
		public static bool TryGetValue<T>(this IBindable bindable, string name, out T value)
		{
			return (value = bindable.GetValue<T>(name)) != null;
		}
	}
}