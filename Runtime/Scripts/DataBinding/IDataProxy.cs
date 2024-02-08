using System;

namespace HHG.Common
{
    public interface IDataProxy<T>
    {
        public T Value { get; set; }

        public IDataProxy<T> Bind(Action<T> bind = null);

        public void Release();
    }
}