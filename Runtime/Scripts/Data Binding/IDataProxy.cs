using System;

namespace HHG.Common.Runtime
{
    public interface IDataProxy
    {
        public object WeakValue { get; set; }
        public IDataProxy WeakBind(Action<object> action);
        public void Release();
    }

    public interface IDataProxy<T> : IDataProxy
    {
        public static readonly IDataProxy<T> Empty = new DataBinding<T>();
        public T Value { get; set; }
        public IDataProxy<T> Bind(Action<T> bind = null);
    }
}