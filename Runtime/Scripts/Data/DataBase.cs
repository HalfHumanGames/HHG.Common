using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class DataBase<TKey, TValue> : MonoBehaviour
    {
        private Dictionary<TKey, TValue> data = new Dictionary<TKey, TValue>();

        public TValue this[TKey key] 
        {
            get => GetData<TValue>(key);
            set => SetData(key, value);
        }

        public bool TryGetData<T>(TKey key, out T value) where T : TValue
        {
            if (data.TryGetValue(key, out TValue weak) && weak is T val)
            {
                value = val;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public TValue GetData(TKey key)
        {
            if (data.TryGetValue(key, out TValue weak))
            {
                return weak;
            }

            return default;
        }

        public T GetData<T>(TKey key) where T : TValue
        {
            if (data.TryGetValue(key, out TValue weak) && weak is T val)
            {
                return val;
            }

            return default;
        }

        public void SetData<T>(TKey key, T value) where T : TValue
        {
            data[key] = value;
            OnSetData(key);
        }

        public void UnsetData(TKey key)
        {
            data.Remove(key);
            OnUnsetData(key);
        }

        protected virtual void OnSetData(TKey key)
        {

        }

        protected virtual void OnUnsetData(TKey key)
        {

        }
    }
}