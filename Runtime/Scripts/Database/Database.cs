using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HHG.Common.Runtime
{
    public static class Database
    {
        private static Dictionary<System.Type, Dictionary<string, Object>> dict = new Dictionary<System.Type, Dictionary<string, Object>>();

        public static bool TryGet<T>(string name, out T val) where T : Object
        {
            val = default;

            if (name == null)
            {
                return false;
            }

            EnsureLoaded<T>();

            System.Type type = typeof(T);

            return dict[type].TryGetValue(name, out Object obj) && (val = obj as T);
        }

        public static T Get<T>(string name) where T : Object
        {
            return TryGet(name, out T val) ? val : default;
        }

        public static T Get<T>() where T : Object
        {
            EnsureLoaded<T>();

            System.Type type = typeof(T);

            return dict[type].Values.OfType<T>().FirstOrDefault();
        }

        public static T Get<T>(System.Func<T, bool> search) where T : Object
        {
            if (search == null)
            {
                return Get<T>();
            }

            EnsureLoaded<T>();

            System.Type type = typeof(T);

            return dict[type].Values.OfType<T>().FirstOrDefault(search);
        }

        public static T[] GetAll<T>() where T : Object
        {
            EnsureLoaded<T>();

            System.Type type = typeof(T);

            return dict[type].Values.OfType<T>().ToArray();
        }

        public static T[] GetAll<T>(System.Func<T, bool> filter) where T : Object
        {
            if (filter == null)
            {
                return GetAll<T>();
            }

            EnsureLoaded<T>();

            System.Type type = typeof(T);

            return dict[type].Values.OfType<T>().Where(filter).ToArray();
        }

        public static bool Contains<T>(string name) where T : Object
        {
            if (name == null)
            {
                return false;
            }

            EnsureLoaded<T>();

            System.Type type = typeof(T);

            return dict[type].ContainsKey(name);
        }

        private static void EnsureLoaded<T>() where T : Object
        {
            System.Type type = typeof(T);

            if (!dict.ContainsKey(type))
            {
                dict[type] = new Dictionary<string, Object>();

                T[] resources = Resources.LoadAll<T>(string.Empty);

                foreach (T item in resources)
                {
                    if (item is not IEnablable enablable || enablable.IsEnabled)
                    {
                        dict[type][item.name] = item;
                    }
                }

#if UNITY_6000_0_OR_NEWER

                string key = typeof(T).Name;

                if (AddressablesUtil.HasKey(key))
                {
                    IList<T> addressables = Addressables.LoadAssetsAsync<T>(key).WaitForCompletion();

                    foreach (T item in addressables)
                    {
                        if (item is not IEnablable enablable || enablable.IsEnabled)
                        {
                            dict[type][item.name] = item;
                        }
                    }
                }

#endif
            }
        }
    }
}