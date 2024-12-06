using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    // Can't have "where T : Component" constraint since that won't work with interfaces
    public class Lazy<T>
    {
        private T component;
        private T[] components;
        public T From(Func<T> getter) => component ??= getter();
        public T FromComponent(Component comp, bool add = false) => component ??= comp.GetOrAddComponent<T>(add);
        public T FromComponent(GameObject gameObject, bool add = false) => component ??= gameObject.GetOrAddComponent<T>(add);
        public T FromComponentInChildren(Component comp, bool includeInactive = false) => component ??= comp.GetComponentInChildren<T>(includeInactive);
        public T FromComponentInChildren(GameObject gameObject, bool includeInactive = false) => component ??= gameObject.GetComponentInChildren<T>(includeInactive);
        public T FromComponentInParent(Component comp, bool includeInactive = false) => component ??= comp.GetComponentInParent<T>(includeInactive);
        public T FromComponentInParent(GameObject gameObject, bool includeInactive = false) => component ??= gameObject.GetComponentInParent<T>(includeInactive);
        public T FromFindObjectOfType(bool includeInactive = false) => component ??= ObjectUtil.FindObjectOfType<T>(includeInactive);
        public T FromGameObjectFind(string name, bool add = false) => component ??= GameObject.Find(name).GetOrAddComponent<T>(add);
        public T FromGameObjectCreate(string name) => component ??= new GameObject(name).GetOrAddComponent<T>(); // Get in case getting Transform
        public T[] From(Func<T[]> getter) => components ??= getter();
        public T[] FromComponentsInChildren(Component comp, bool includeInactive = false) => components ??= comp.GetComponentsInChildren<T>(includeInactive);
        public T[] FromComponentsInChildren(GameObject gameObject, bool includeInactive = false) => components ??= gameObject.GetComponentsInChildren<T>(includeInactive);
        public T[] FromComponentsInParent(Component comp, bool includeInactive = false) => components ??= comp.GetComponentsInParent<T>(includeInactive);
        public T[] FromComponentsInParent(GameObject gameObject, bool includeInactive = false) => components ??= gameObject.GetComponentsInParent<T>(includeInactive);
        public T[] FromComponents(Component comp) => components ??= comp.GetComponents<T>();
        public T[] FromComponents(GameObject gameObject) => components ??= gameObject.GetComponents<T>();
        public T[] FromFindObjectsOfType(bool includeInactive = false) => components ??= ObjectUtil.FindObjectsOfType<T>(includeInactive);

        public void Reset()
        {
            component = default;
            components = default;
        }
    }
}