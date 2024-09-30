using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HHG.Common.Runtime
{
    public static class Database
    {
        private static Dictionary<Type, Dictionary<string, Object>> dict = new Dictionary<Type, Dictionary<string, Object>>();

        public static bool TryGet<T>(string name, out T val) where T : Object
        {
            val = default;

            if (name == null)
            {
                return false;
            }

            EnsureLoaded<T>();

            Type key = typeof(T);

            return dict[key].TryGetValue(name, out Object obj) && (val = obj as T);
        }

        public static T Get<T>(string name) where T : Object
        {
            return TryGet(name, out T val) ? val : default;
        }

        public static T Get<T>() where T : Object
        {
            EnsureLoaded<T>();

            Type key = typeof(T);

            return dict[key].Values.OfType<T>().FirstOrDefault();
        }

        public static T Get<T>(Func<T, bool> search) where T : Object
        {
            if (search == null)
            {
                return Get<T>();
            }

            EnsureLoaded<T>();

            Type key = typeof(T);

            return dict[key].Values.OfType<T>().FirstOrDefault(search);
        }

        public static T[] GetAll<T>() where T : Object
        {
            EnsureLoaded<T>();

            Type key = typeof(T);

            return dict[key].Values.OfType<T>().ToArray();
        }

        public static T[] GetAll<T>(Func<T, bool> filter) where T : Object
        {
            if (filter == null)
            {
                return GetAll<T>();
            }

            EnsureLoaded<T>();

            Type key = typeof(T);

            return dict[key].Values.OfType<T>().Where(filter).ToArray();
        }

        public static bool Contains<T>(string name) where T : Object
        {
            if (name == null)
            {
                return false;
            }

            EnsureLoaded<T>();

            Type key = typeof(T);

            return dict[key].ContainsKey(name);
        }

        private static void EnsureLoaded<T>() where T : Object
        {
            Type key = typeof(T);

            if (!dict.ContainsKey(key))
            {
                dict[key] = new Dictionary<string, Object>();

                T[] items = Resources.LoadAll<T>(string.Empty);

                foreach (T item in items)
                {
                    if (item is not IEnablable enablable || enablable.IsEnabled)
                    {
                        dict[key][item.name] = item;
                    }
                }
            }
        }
    }
}