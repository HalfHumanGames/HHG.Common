using UnityEngine;

namespace HHG.Common
{
    public static class MonobehaviourExtensions
    {
        public static bool TryGetComponentInChildren<T>(this MonoBehaviour mono, out T component) where T : Component
        {
           return mono.gameObject.TryGetComponentInChildren(out component);
        }

        public static bool TryGetComponentInParent<T>(this MonoBehaviour mono, out T component) where T : Component
        {
            return mono.gameObject.TryGetComponentInParent(out component);
        }

        public static T GetOrAddComponent<T>(this MonoBehaviour mono) where T : Component
        {
            return mono.gameObject.GetOrAddComponent<T>();
        }

        public static T GetTopmostComponent<T>(this MonoBehaviour mono) where T : Component
        {
            return mono.gameObject.GetTopmostComponent<T>();
        }
    }
}