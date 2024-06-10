using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ComponentExtensions
    {
        public static bool TryGetComponentInChildren<T>(this Component mono, out T component) 
        {
           return mono.gameObject.TryGetComponentInChildren(out component);
        }

        public static bool TryGetComponentInParent<T>(this Component mono, out T component) 
        {
            return mono.gameObject.TryGetComponentInParent(out component);
        }

        public static T GetComponent<T>(this Component mono)
        {
            return mono.gameObject.GetComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component mono, bool add = true)
        {
            return mono.gameObject.GetOrAddComponent<T>(add);
        }

        public static T GetTopmostComponent<T>(this Component mono) 
        {
            return mono.gameObject.GetTopmostComponent<T>();
        }
    }
}