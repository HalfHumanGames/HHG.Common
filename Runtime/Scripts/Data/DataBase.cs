using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common
{
    public abstract class DataBase<TKey, TValue> : MonoBehaviour
    {
        private Dictionary<TKey, TValue> data = new Dictionary<TKey, TValue>();

        public TValue this[TKey key] 
        {
            get => GetData<TValue>(key);
            set => SetData(key, value);
        }

        public bool TryGetData<T>(TKey position, out T value) where T : TValue
        {
            if (data.TryGetValue(position, out TValue weak) && weak is T val)
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

        public TValue GetData(TKey position)
        {
            if (data.TryGetValue(position, out TValue weak))
            {
                return weak;
            }

            return default;
        }

        public T GetData<T>(TKey position) where T : TValue
        {
            if (data.TryGetValue(position, out TValue weak) && weak is T val)
            {
                return val;
            }

            return default;
        }

        public void SetData<T>(TKey position, T value) where T : TValue
        {
            data[position] = value;
            OnSetData(position);
        }

        public void UnsetData(TKey position)
        {
            data.Remove(position);
            OnUnsetData(position);
        }

        protected virtual void OnSetData(TKey position)
        {

        }

        protected virtual void OnUnsetData(TKey position)
        {

        }
    }
}