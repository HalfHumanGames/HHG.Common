using System;
using System.Text;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class GameObjectExtensions
    {
        public static string GetPath(this GameObject gameObject)
        {
            StringBuilder sb = new StringBuilder($"/{gameObject.name}");
            while (gameObject.transform.parent != null)
            {
                gameObject = gameObject.transform.parent.gameObject;
                sb.Insert(0, $"/{gameObject.name}");
            }
            return sb.ToString();
        }

        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component, bool includeInactive = false)
        {
            component = gameObject.GetComponentInChildren<T>(includeInactive);
            return component != null;
        }

        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component, bool includeInactive = false)
        {
            component = gameObject.GetComponentInParent<T>(includeInactive);
            return component != null;
        }

        public static T GetOrAddComponent<T>(this GameObject go, bool add = true)
        {
            if (!go.TryGetComponent(typeof(T), out Component component) && add)
            {
                component = go.AddComponent(typeof(T));
            }
            return (T)(object)component;
        }

        public static T GetTopmostComponent<T>(this GameObject gameObject, bool includeInactive = false)
        {
            T component = gameObject.GetComponentInParent<T>(includeInactive);

            while (true)
            {
                Transform parent = (component as Component).transform.parent;

                if (parent == null)
                {
                    break;
                }

                T parentComponent = parent.GetComponentInParent<T>(includeInactive);

                if (parentComponent == null)
                {
                    break;
                }

                component = parentComponent;
            }

            return component;
        }
    }
}