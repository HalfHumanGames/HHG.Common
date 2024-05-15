using System.Collections.Generic;
using System.Reflection;
using System;

namespace HHG.Common.Runtime
{
    public class GetSetMap
    {
        private Dictionary<string, Func<object, object>> getters;
        private Dictionary<string, Action<object, object>> setters;

        public GetSetMap(Type type)
        {
            getters = new Dictionary<string, Func<object, object>>();
            setters = new Dictionary<string, Action<object, object>>();
            PropertyInfo[] props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo prop in props)
            {
                getters[prop.Name] = obj => prop.GetValue(obj);
                setters[prop.Name] = (obj, value) => prop.SetValue(obj, value);
            }
        }

        public bool TryGetValue<T>(object obj, string name, out T value)
        {
            if (getters.TryGetValue(name, out Func<object, object> func) && func(obj) is T val)
            {
                value = val;
                return true;
            }

            value = default;
            return false;
        }

        public bool TrySetValue<T>(object obj, string name, T value)
        {
            if (setters.TryGetValue(name, out Action<object, object> func))
            {
                func(obj, value);
                return true;
            }

            return false;
        }
    }
}