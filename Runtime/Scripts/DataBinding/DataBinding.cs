using System;

namespace HHG.Common
{
    public class DataBinding<TMember>
    {
        private readonly DataProxy<TMember> proxy;
        private readonly Action<TMember> setter;

        public DataBinding(DataProxy<TMember> source, Action<TMember> set)
        {
            proxy = source;
            setter = set;
        }

        public void Set(TMember value)
        {
            setter(value);
        }

        public void Release()
        {
            proxy.Unbind(this);
        }
    }
}