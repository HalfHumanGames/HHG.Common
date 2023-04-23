using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Database
{
    private static Dictionary<System.Type, Dictionary<string, Object>> dict = new Dictionary<System.Type, Dictionary<string, Object>>();

    public static T Get<T>(string name) where T : Object
    {
        EnsureLoaded<T>();
        System.Type key = typeof(T);
        return dict[key].ContainsKey(name) ? dict[key][name] as T : default; 
    }

    public static T Get<T>(System.Func<T, bool> search) where T : Object
    {
        EnsureLoaded<T>();
        return GetAll<T>().FirstOrDefault(search);
    }

    public static T[] GetAll<T>(System.Func<T, bool> filter = null) where T : Object
    {
        EnsureLoaded<T>();
        System.Type key = typeof(T);
        filter = filter ?? (item => true);
        return dict[key].Values.Select(item => item as T).Where(filter).ToArray();
    }

    public static bool Contains<T>(string name) where T : Object
    {
        EnsureLoaded<T>();
        System.Type key = typeof(T);
        return dict[key].ContainsKey(name);
    }

    private static void EnsureLoaded<T>() where T : Object
    {
        System.Type key = typeof(T);
        if (dict.ContainsKey(key)) return;
        dict[key] = new Dictionary<string, Object>();
        T[] items = Resources.LoadAll<T>("");
        foreach (T item in items)
        {
            dict[key][item.name] = item;
        }
    }
}