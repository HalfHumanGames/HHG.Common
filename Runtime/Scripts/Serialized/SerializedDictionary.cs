using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IList<KeyValuePair<TKey, TValue>>, IList, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new List<TKey>();
        [SerializeField] private List<TValue> values = new List<TValue>();

        [SerializeField, HideInInspector] private List<int> toAdd = new List<int>();
        [SerializeField, HideInInspector] private List<TKey> keysToAdd = new List<TKey>();
        [SerializeField, HideInInspector] private List<TValue> valuesToAdd = new List<TValue>();

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (value is KeyValuePair<TKey, TValue> kvpair)
                {
                    this[index] = kvpair;
                }
            }
        }

        public KeyValuePair<TKey, TValue> this[int index]
        {
            get
            {
                OnBeforeSerialize();
                return new KeyValuePair<TKey, TValue>(keys[index], values[index]);
            }
            set
            {
                OnBeforeSerialize();
                keys[index] = value.Key;
                values[index] = value.Value;
                OnAfterDeserialize();
            }
        }

        public SerializedDictionary()
        {

        }

        public SerializedDictionary(IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            this.keys.Clear();
            this.keys.AddRange(keys);
            this.values.Clear();
            this.values.AddRange(values);

            OnAfterDeserialize();
        }

        public int Add(object item)
        {
            if (item is KeyValuePair<TKey, TValue> kvpair)
            {
                OnBeforeSerialize();
                keys.Add(kvpair.Key);
                values.Add(kvpair.Value);
                OnAfterDeserialize();
                return keys.Count - 1;
            }
            else
            {
                return -1;
            }
        }

        public bool Contains(object item)
        {
            if (item is KeyValuePair<TKey, TValue> kvpair)
            {
                OnBeforeSerialize();
                for (int i = 0; i < keys.Count; i++)
                {
                    if (keys[i].Equals(kvpair.Key) && values[i].Equals(kvpair.Value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public int IndexOf(object item)
        {
            if (item is KeyValuePair<TKey, TValue> kvpair)
            {
                return IndexOf(kvpair);
            }

            return -1;
        }

        public void Insert(int index, object item)
        {
            if (item is KeyValuePair<TKey, TValue> kvpair)
            {
                Insert(index, kvpair);
            }
        }

        public int IndexOf(KeyValuePair<TKey, TValue> item)
        {
            OnBeforeSerialize();
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(item.Key) && values[i].Equals(item.Value))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, KeyValuePair<TKey, TValue> item)
        {
            OnBeforeSerialize();
            keys.Insert(index, item.Key);
            values.Insert(index, item.Value);
            OnAfterDeserialize();
        }

        public void Remove(object item)
        {
            if (item is KeyValuePair<TKey, TValue> kvpair)
            {
                OnBeforeSerialize();
                for (int i = 0; i < keys.Count; i++)
                {
                    if (keys[i].Equals(kvpair.Key) && values[i].Equals(kvpair.Value))
                    {
                        keys.RemoveAt(i);
                        values.RemoveAt(i);
                        OnAfterDeserialize();
                        return;
                    }
                }
            }
        }

        public void RemoveAt(int index)
        {
            OnBeforeSerialize();
            keys.RemoveAt(index);
            values.RemoveAt(index);
            OnAfterDeserialize();
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            int i = 0;
            int added = 0;
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                if (toAdd.Contains(i))
                {
                    int index = toAdd.IndexOf(i);
                    keys.Add(keysToAdd[index]);
                    values.Add(valuesToAdd[index]);
                    added++;
                    i++;
                }

                keys.Add(pair.Key);
                values.Add(pair.Value);
                i++;
            }

            for (i = added; i < toAdd.Count; i++)
            {
                keys.Add(keysToAdd[i]);
                values.Add(valuesToAdd[i]);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            toAdd.Clear();
            keysToAdd.Clear();
            valuesToAdd.Clear();

            if (keys.Count < values.Count)
            {
                keys.Resize(values.Count);
            }
            else if (values.Count < keys.Count)
            {
                values.Resize(keys.Count);
            }

            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] == null || !TryAdd(keys[i], values[i]))
                {
                    toAdd.Add(i);
                    keysToAdd.Add(keys[i]);
                    valuesToAdd.Add(values[i]);
                }
            }
        }
    }
}