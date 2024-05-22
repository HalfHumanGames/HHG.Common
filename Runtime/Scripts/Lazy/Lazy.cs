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
        public T FromComponent(Component comp) => component = comp.GetComponent<T>();
        public T FromComponent(GameObject gameObject) => component = gameObject.GetComponent<T>();
        public T FromComponentInChildren(Component comp) => component ??= comp.GetComponentInChildren<T>();
        public T FromComponentInChildren(GameObject gameObject) => component ??= gameObject.GetComponentInChildren<T>();
        public T FromComponentInParent(Component comp) => component ??= comp.GetComponentInParent<T>();
        public T FromComponentInParent(GameObject gameObject) => component ??= gameObject.GetComponentInParent<T>();
        public T FromFindObjectOfType() => component ??= ObjectUtil.FindObjectOfType<T>();
        public T[] From(Func<T[]> getter) => components ??= getter();
        public T[] FromComponentsInChildren(Component comp) => components ??= comp.GetComponentsInChildren<T>();
        public T[] FromComponentsInChildren(GameObject gameObject) => components ??= gameObject.GetComponentsInChildren<T>();
        public T[] FromComponentsInParent(Component comp) => components ??= comp.GetComponentsInParent<T>();
        public T[] FromComponentsInParent(GameObject gameObject) => components ??= gameObject.GetComponentsInParent<T>();
        public T[] FromComponents(Component comp) => components ??= comp.GetComponents<T>();
        public T[] FromComponents(GameObject gameObject) => components ??= gameObject.GetComponents<T>();
        public T[] FromFindObjectsOfType() => components ??= ObjectUtil.FindObjectsOfType<T>();
    }
}