using System.Text;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class GameObjectExtensions
    {
        public static string GetPath(this GameObject go)
        {
            StringBuilder sb = new StringBuilder($"/{go.name}");
            while (go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
                sb.Insert(0, $"/{go.name}");
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

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (!go.TryGetComponent(out T component))
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        public static T GetTopmostComponent<T>(this GameObject go, bool includeInactive = false)
        {
            T component = go.GetComponentInParent<T>(includeInactive);

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