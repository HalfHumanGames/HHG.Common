using System;

namespace HHG.Common
{
    public class DataBinding<TMember>
    {
        private readonly DataProxy<TMember> proxy;
        private readonly Action<TMember> setter;

        public TMember Value
        {
            get => proxy;
            set => proxy.Value = value;
        }

        internal DataBinding(DataProxy<TMember> source, Action<TMember> set = null)
        {
            proxy = source;
            setter = set ?? (v => { });
        }

        internal void Set(TMember value) => setter(value);

        public void Release() => proxy.Unbind(this);

        public static implicit operator TMember(DataBinding<TMember> binding) => binding.proxy;
    }
}