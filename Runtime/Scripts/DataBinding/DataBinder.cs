using System;

namespace HHG.Common
{
    public class DataBinder
    {
        public static DataProxy<TMember> CreateProxy<TMember>(Func<TMember> get, Action<TMember> set)
        {
            return new DataProxy<TMember>(get, set);
        }

        public static DataBinding<TMember> Bind<TMember>(DataProxy<TMember> proxy, Action<TMember> bind)
        {
            DataBinding<TMember> binding = new DataBinding<TMember>(proxy, bind);
            proxy.bindings.Add(binding);
            return binding;
        }
    }
}