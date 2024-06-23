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

        public static T Get<T>(string name) where T : Object
        {
            EnsureLoaded<T>();
            Type key = typeof(T);
            return dict[key].ContainsKey(name) ? dict[key][name] as T : default;
        }

        public static T Get<T>(Func<T, bool> search = null) where T : Object
        {
            search ??= _ => true;
            EnsureLoaded<T>();
            return GetAll<T>().FirstOrDefault(search);
        }

        public static T[] GetAll<T>(Func<T, bool> filter = null) where T : Object
        {
            EnsureLoaded<T>();
            Type key = typeof(T);
            filter ??= _ => true;
            return dict[key].Values.Select(item => item as T).Where(filter).ToArray();
        }

        public static bool Contains<T>(string name) where T : Object
        {
            EnsureLoaded<T>();
            Type key = typeof(T);
            return dict[key].ContainsKey(name);
        }

        private static void EnsureLoaded<T>() where T : Object
        {
            Type key = typeof(T);
            if (dict.ContainsKey(key)) return;
            dict[key] = new Dictionary<string, Object>();
            T[] items = Resources.LoadAll<T>("");
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