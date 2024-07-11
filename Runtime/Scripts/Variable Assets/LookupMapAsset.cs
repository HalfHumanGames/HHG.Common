using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class LookupMapAsset<T> : LookupMapAsset<T, T>
    {
        
    }

    public abstract class LookupMapAsset<K, V>: ScriptableObject
    {
        public IReadOnlyDictionary<K, V> Map => map;

        [SerializeField] private SerializableDictionary<K, V> map = new SerializableDictionary<K, V>();
    }
}