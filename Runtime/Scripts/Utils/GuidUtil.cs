using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace HHG.Common.Runtime
{
    public static class GuidUtil
    {
        public static string EnsureUnique<T>(T source, Func<T, string> func) where T : Object
        {
            string guid = func(source);

#if UNITY_EDITOR

            if (!PrefabUtility.IsPartOfPrefabAsset(source))
            {
                Func<T, bool> search = obj => func(obj) == guid && obj != source;
                T dupe = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault(search);
                return dupe ? Guid.NewGuid().ToString() : guid;
            }

#endif
            return guid; // Prefabs always keep their id
        }
    }
}