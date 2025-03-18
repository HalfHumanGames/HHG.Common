namespace HHG.Common.Runtime
{
    public interface IUpdated
    {
        //T GetValue<T>(string name);
        //void SetValue<T>(string name, T value);
        event System.Action Updated;
    }

    //public static class IBindableExtensions
    //{
    //    public static object GetValue(this IUpdateListener bindable, string name)
    //    {
    //        return bindable.GetValue<object>(name);
    //    }

    //    public static bool TryGetValue(this IUpdateListener bindable, string name, out object value)
    //    {
    //        return (value = bindable.GetValue(name)) != null;
    //    }

    //    public static bool TryGetValue<T>(this IUpdateListener bindable, string name, out T value)
    // {
    //  return (value = bindable.GetValue<T>(name)) != null;
    // }
    //}
}