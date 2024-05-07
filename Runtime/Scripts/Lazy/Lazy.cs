using UnityEngine;

namespace HHG.Common.Runtime
{
    // Can't have "where T : Component" constraint since that won't work with interfaces
    public class Lazy<T> 
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

        public T FromFindObjectOfType(MonoBehaviour mono)
        {
            if (component == null)
            {
                component = mono.FindObjectOfType<T>();
            }

            return component;
        }

        public T[] FromFindObjectsOfType(MonoBehaviour mono)
        {
            if (components == null)
            {
                components = mono.FindObjectsOfType<T>();
            }

            return components;
        }
    }
}