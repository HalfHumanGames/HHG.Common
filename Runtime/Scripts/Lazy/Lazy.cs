using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    // Can't have "where T : Component" constraint since that won't work with interfaces
    public class Lazy<T>
    {
        private T component;
        private T[] components;
        public T From(System.Func<T> getter) => component ??= getter();
        public T FromComponent(Component comp, bool add = false) => component ??= comp.GetOrAddComponent<T>(add);
        public T FromComponent(GameObject gameObject, bool add = false) => component ??= gameObject.GetOrAddComponent<T>(add);
        public T FromComponentInChildren(Component comp, bool includeInactive = false) => component ??= comp.GetComponentInChildren<T>(includeInactive);
        public T FromComponentInChildren(GameObject gameObject, bool includeInactive = false) => component ??= gameObject.GetComponentInChildren<T>(includeInactive);
        public T FromComponentInParent(Component comp, bool includeInactive = false) => component ??= comp.GetComponentInParent<T>(includeInactive);
        public T FromComponentInParent(GameObject gameObject, bool includeInactive = false) => component ??= gameObject.GetComponentInParent<T>(includeInactive);
        public T FromComponentInScene(bool includeInactive = false) => component ??= ObjectUtil.FindComponentInScene<T>(includeInactive);
        public T FromObjectByType(bool includeInactive = false) => component ??= (T)(object)Object.FindAnyObjectByType(typeof(T), includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);
        public T FromFind(string name, bool add = false) => component ??= GameObject.Find(name).GetOrAddComponent<T>(add);
        public T FromFindOrCreate(string name) => component ??= new GameObject(name).GetOrAddComponent<T>(); // Get in case getting Transform
        public T[] From(System.Func<T[]> getter) => components ??= getter();
        public T[] FromComponentsInChildren(Component comp, bool includeInactive = false) => components ??= comp.GetComponentsInChildren<T>(includeInactive);
        public T[] FromComponentsInChildren(GameObject gameObject, bool includeInactive = false) => components ??= gameObject.GetComponentsInChildren<T>(includeInactive);
        public T[] FromComponentsInParent(Component comp, bool includeInactive = false) => components ??= comp.GetComponentsInParent<T>(includeInactive);
        public T[] FromComponentsInParent(GameObject gameObject, bool includeInactive = false) => components ??= gameObject.GetComponentsInParent<T>(includeInactive);
        public T[] FromComponents(Component comp) => components ??= comp.GetComponents<T>();
        public T[] FromComponents(GameObject gameObject) => components ??= gameObject.GetComponents<T>();
        public T[] FindComponentsInScene(bool includeInactive = false) => components ??= ObjectUtil.FindComponentsInScene<T>(includeInactive);
        public T[] FromObjectsByType(bool includeInactive = false) => components ??= Object.FindObjectsByType(typeof(T), includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, default).Cast<T>().ToArray();

        public void Reset()
        {
            component = default;
            components = default;
        }
    }
}