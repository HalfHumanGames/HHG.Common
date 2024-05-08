using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HHG.Common.Runtime
{
    public static class GuidUtil
    {
        public static string EnsureUnique<T>(T source, Func<T, string> func) where T : Object
        {
            string id = func(source);
            T dupe = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault(obj => func(obj) == id && obj != source);
            return dupe ? Guid.NewGuid().ToString() : id;
        }
    }
}