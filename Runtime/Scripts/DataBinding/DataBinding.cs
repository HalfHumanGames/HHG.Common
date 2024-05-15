using System;

namespace HHG.Common.Runtime
{
    internal abstract class DataBinding : IDataProxy
    {
        public abstract object WeakValue { get; set; }

        public abstract IDataProxy WeakBind(Action<object> action);

        public abstract void Release();
    }

    internal class DataBinding<T> : DataBinding, IDataProxy<T>
    {
        private readonly DataProxy<T> proxy;
        private readonly Action<T> setter;

        public T Value
        {
            get => proxy;
            set
            {
                if (proxy != null)
                {
                    proxy.Value = value;
                }
            }
        }

        public override object WeakValue
        {
            get => proxy;
            set
            {
                if (proxy != null)
                {
                    proxy.Value = (T)value;
                }
            }
        }

        internal DataBinding(DataProxy<T> source = null, Action<T> set = null)
        {
            proxy = source;
            setter = set;
        }

        internal void Set(T value) => setter?.Invoke(value);

        public override IDataProxy WeakBind(Action<object> bind = null) => proxy?.Bind(o => bind(o));

        public IDataProxy<T> Bind(Action<T> bind = null) => proxy?.Bind(bind);

        public override void Release() => proxy?.Unbind(this);

        public static implicit operator T(DataBinding<T> binding) => binding.proxy;
    }
}