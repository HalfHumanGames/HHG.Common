using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class LookupMapAsset<T> : LookupMapAsset<T, T>
    {
        protected override void SwapKeyValue()
        {
            map = new SerializedDictionary<T, T>(map.Values, map.Keys);
        }
    }

    public abstract class LookupMapAsset<K, V>: LookupMapAsset
    {
        public IReadOnlyDictionary<K, V> Map => map;

        [SerializeField] protected SerializedDictionary<K, V> map = new SerializedDictionary<K, V>();
    }

    public abstract class LookupMapAsset : ScriptableObject
    {
        [ContextMenu("Swap Key Value")]
        protected virtual void SwapKeyValue()
        {
            SwapKeyValue(); // This calls the subclass implemetation
        }
    }
}