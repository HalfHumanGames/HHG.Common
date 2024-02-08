using System;
using System.Collections.Generic;

namespace HHG.Common
{
    public class DataProxy<T>
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

        public DataBinding<T> Bind(Action<T> bind = null)
        {
            DataBinding<T> binding = new DataBinding<T>(this, bind);
            binding.Set(getter());
            bindings.Add(binding);
            return binding;
        }

        public void Unbind(DataBinding<T> binding) => bindings.Remove(binding);

        public static implicit operator T(DataProxy<T> proxy) => proxy == null ? default : proxy.getter();
    }
}