using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public sealed class MetaData : MonoBehaviour
    {
        private readonly Dictionary<object, object> values = new Dictionary<object, object>();

        public void Set(object key, object value)
        {
            values[key] = value;
        }

        public TValue Get<TValue>(object key)
        {
            return (TValue)values[key];
        }

        public bool TryGet<TValue>(object key, out TValue value)
        {
            if (values.TryGetValue(key, out object raw))
            {
                value = (TValue)raw;
                return true;
            }

            value = default;
            return false;
        }

        public bool Has(object key)
        {
            return values.ContainsKey(key);
        }

        public bool Remove(object key)
        {
            return values.Remove(key);
        }
    }
}
