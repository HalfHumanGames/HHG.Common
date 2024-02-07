using System;
using System.Collections.Generic;

namespace HHG.Common
{
    public class DataProxy<TMember>
    {
        private readonly Func<TMember> getter;
        private readonly Action<TMember> setter;

        internal readonly List<DataBinding<TMember>> bindings = new List<DataBinding<TMember>>();

        public TMember Value
        {
            get => getter();
            set
            {
                setter(value);
                foreach (DataBinding<TMember> binding in bindings)
                {
                    binding.Set(value);
                }
            }
        }

        internal DataProxy(Func<TMember> get, Action<TMember> set)
        {
            getter = get;
            setter = set;
        }

        public static implicit operator TMember(DataProxy<TMember> dataBinding)
        {
            return dataBinding.getter();
        }
    }
}