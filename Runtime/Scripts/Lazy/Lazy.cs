using UnityEngine;

namespace HHG.Common
{
    public class Lazy<T> where T : Component
    {
        private T component;
        private T[] components;

        public T FromComponent(MonoBehaviour mono)
        {
            if (component == null)
            {
                component = mono.GetComponent<T>();
            }

            return component;
        }

        public T[] FromComponents(MonoBehaviour mono)
        {
            if (components == null)
            {
                components = mono.GetComponents<T>();
            }

            return components;
        }

        public T FromComponentInChildren(MonoBehaviour mono)
        {
            if (component == null)
            {
                component = mono.GetComponentInChildren<T>();
            }

            return component;
        }

        public T[] FromComponentsInChildren(MonoBehaviour mono)
        {
            if (components == null)
            {
                components = mono.GetComponentsInChildren<T>();
            }

            return components;
        }

        public T FromComponentInParent(MonoBehaviour mono)
        {
            if (component == null)
            {
                component = mono.GetComponentInParent<T>();
            }

            return component;
        }

        public T[] FromComponentsInParent(MonoBehaviour mono)
        {
            if (components == null)
            {
                components = mono.GetComponentsInParent<T>();
            }

            return components;
        }
    }
}