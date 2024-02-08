using System;

namespace HHG.Common
{
    internal class DataBinding<T> : IDataProxy<T>
    {
        private readonly DataProxy<T> proxy;
        private readonly Action<T> setter;

        public T Value
        {
            get => proxy;
            set {
                if (proxy != null)
                {
                    proxy.Value = value;
                }
            }
        }

        internal DataBinding(DataProxy<T> source = null, Action<T> set = null)
        {
            proxy = source;
            setter = set;
        }

        internal void Set(T value) => setter?.Invoke(value);

        public IDataProxy<T> Bind(Action<T> bind = null) => proxy?.Bind(bind);

        public void Release() => proxy?.Unbind(this);

        public static implicit operator T(DataBinding<T> binding) => binding.proxy;
    }
}