using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    // Can't have "where T : Component" constraint since that won't work with interfaces
    public class Lazy<T>
    {
        public T Value => value;

        private T value;

        public T From(System.Func<T> getter) => value ??= getter();
        public T FromComponent(Component comp, bool add = false) => value ??= comp.GetOrAddComponent<T>(add);
        public T FromComponent(GameObject gameObject, bool add = false) => value ??= gameObject.GetOrAddComponent<T>(add);
        public T FromComponentInChildren(Component comp, bool includeInactive = false) => value ??= comp.GetComponentInChildren<T>(includeInactive);
        public T FromComponentInChildren(GameObject gameObject, bool includeInactive = false) => value ??= gameObject.GetComponentInChildren<T>(includeInactive);
        public T FromComponentInParent(Component comp, bool includeInactive = false) => value ??= comp.GetComponentInParent<T>(includeInactive);
        public T FromComponentInParent(GameObject gameObject, bool includeInactive = false) => value ??= gameObject.GetComponentInParent<T>(includeInactive);
        public T FromComponentInScene(bool includeInactive = false) => value ??= ObjectUtil.FindComponentInScene<T>(includeInactive);
        public T FromObjectByType(bool includeInactive = false) => value ??= (T)(object)Object.FindAnyObjectByType(typeof(T), includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);
        public T FromFind(string name, bool add = false) => value ??= GameObject.Find(name).GetOrAddComponent<T>(add);
        public T FromFindOrCreate(string name) => value ??= new GameObject(name).GetOrAddComponent<T>(); // Get in case getting Transform
        public T FromLocator(string id = null) => value ??= Locator.Get<T>(id);

        public void Clear()
        {
            value = default;
        }
    }

    public class LazyArray<T>
    {
        public T[] Values => values;
        private T[] values;

        public T[] From(System.Func<T[]> getter) => values ??= getter();
        public T[] FromComponentsInChildren(Component comp, bool includeInactive = false) => values ??= comp.GetComponentsInChildren<T>(includeInactive);
        public T[] FromComponentsInChildren(GameObject gameObject, bool includeInactive = false) => values ??= gameObject.GetComponentsInChildren<T>(includeInactive);
        public T[] FromComponentsInParent(Component comp, bool includeInactive = false) => values ??= comp.GetComponentsInParent<T>(includeInactive);
        public T[] FromComponentsInParent(GameObject gameObject, bool includeInactive = false) => values ??= gameObject.GetComponentsInParent<T>(includeInactive);
        public T[] FromComponents(Component comp) => values ??= comp.GetComponents<T>();
        public T[] FromComponents(GameObject gameObject) => values ??= gameObject.GetComponents<T>();
        public T[] FindComponentsInScene(bool includeInactive = false) => values ??= ObjectUtil.FindComponentsInScene<T>(includeInactive);
        public T[] FromObjectsByType(bool includeInactive = false) => values ??= Object.FindObjectsByType(typeof(T), includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, default).Cast<T>().ToArray();

        public void Clear()
        {
            values = default;
        }
    }
}