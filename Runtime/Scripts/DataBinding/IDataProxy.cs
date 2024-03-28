using System;

namespace HHG.Common.Runtime
{
    public interface IDataProxy<T>
    {
        public static readonly IDataProxy<T> Empty = new DataBinding<T>();

        public T Value { get; set; }

        public IDataProxy<T> Bind(Action<T> bind = null);

        public void Release();
    }
}