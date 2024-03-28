using System;
using System.Collections.Generic;

namespace HHG.Common.Runtime
{
    public class DataProxy<T> : IDataProxy<T>
    {
        private readonly Func<T> getter;
        private readonly Action<T> setter;
        private readonly List<DataBinding<T>> bindings = new List<DataBinding<T>>();

        public T Value
        {
            get => getter();
            set
            {
                setter(value);
                foreach (DataBinding<T> binding in bindings)
                {
                    binding.Set(value);
                }
            }
        }

        public DataProxy(Func<T> get, Action<T> set)
        {
            getter = get;
            setter = set;
        }

        public IDataProxy<T> Bind(Action<T> bind = null)
        {
            DataBinding<T> binding = new DataBinding<T>(this, bind);
            binding.Set(getter());
            bindings.Add(binding);
            return binding;
        }

        internal void Unbind(DataBinding<T> binding) => bindings.Remove(binding);

        public void Release() { }

        public override string ToString() => Value.ToString();

        public static implicit operator T(DataProxy<T> proxy) => proxy == null ? default : proxy.getter();
    }
}