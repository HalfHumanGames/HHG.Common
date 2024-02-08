using System;

namespace HHG.Common
{
    public class DataBinding<TMember>
    {
        private readonly DataProxy<TMember> proxy;
        private readonly Action<TMember> setter;

        internal DataBinding(DataProxy<TMember> source, Action<TMember> set = null)
        {
            proxy = source;
            setter = set ?? (v => { });
        }

        public void Set(TMember value) => setter(value);

        public void Release() => proxy.Unbind(this);

        public static implicit operator TMember(DataBinding<TMember> binding) => binding.proxy;
    }
}