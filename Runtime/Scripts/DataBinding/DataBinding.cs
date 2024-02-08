using System;

namespace HHG.Common
{
    public class DataBinding<TMember>
    {
        public static readonly DataBinding<TMember> Empty = new DataBinding<TMember>();

        private readonly DataProxy<TMember> proxy;
        private readonly Action<TMember> setter;

        public TMember Value
        {
            get => proxy;
            set {
                if (proxy != null)
                {
                    proxy.Value = value;
                }
            }
        }

        internal DataBinding(DataProxy<TMember> source = null, Action<TMember> set = null)
        {
            proxy = source;
            setter = set;
        }

        internal void Set(TMember value) => setter?.Invoke(value);

        public void Release() => proxy?.Unbind(this);

        public static implicit operator TMember(DataBinding<TMember> binding) => binding.proxy;
    }
}