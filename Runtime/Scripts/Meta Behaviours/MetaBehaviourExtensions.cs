using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class MetaBehaviourExtensions
    {
        public static T AddMetaBehaviour<T>(this GameObject gameObject, T metaBehaviour) where T : MetaBehaviour
        {
            gameObject.SetActive(false);
            MetaBehaviour instance = gameObject.AddComponent<MetaBehaviourRunner>().AttachBehaviour(metaBehaviour.Clone());
            gameObject.SetActive(true);
            return (T)instance;
        }

        public static T[] AddMetaBehaviours<T>(this GameObject gameObject, IEnumerable<T> metaBehaviours) where T : MetaBehaviour
        {
            gameObject.SetActive(false);
            int count = metaBehaviours.Count();
            T[] instances = new T[count];
            int i = 0;
            foreach (T metaBehaviour in metaBehaviours)
            {
                instances[i++] = (T)gameObject.AddComponent<MetaBehaviourRunner>().AttachBehaviour(metaBehaviour.Clone());
            }
            gameObject.SetActive(true);
            return instances;
        }

        public static T GetMetaBehaviour<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            return (T) gameObject.GetMetaBehaviour(typeof(T));
        }

        public static MetaBehaviour GetMetaBehaviour(this GameObject gameObject, Type type) 
        {
            return gameObject.GetComponents<MetaBehaviourRunner>().FirstOrDefault(r => r.Behaviour.GetType() == type)?.Behaviour;
        }

        public static T GetMetaBehaviourInChildren<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            return (T)gameObject.GetMetaBehaviourInChildren(typeof(T));
        }

        public static MetaBehaviour GetMetaBehaviourInChildren(this GameObject gameObject, Type type)
        {
            return gameObject.GetComponentsInChildren<MetaBehaviourRunner>().FirstOrDefault(r => r.Behaviour.GetType() == type)?.Behaviour;
        }

        public static T GetMetaBehaviourInParent<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            return (T)gameObject.GetMetaBehaviourInParent(typeof(T));
        }

        public static MetaBehaviour GetMetaBehaviourInParent(this GameObject gameObject, Type type)
        {
            return gameObject.GetComponentsInParent<MetaBehaviourRunner>().FirstOrDefault(r => r.Behaviour.GetType() == type)?.Behaviour;
        }

        public static T[] GetMetaBehaviours<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponents<MetaBehaviourRunner>();
            return runners.Where(r => r.Behaviour.GetType() == typeof(T)).Select(r => (T)r.Behaviour).ToArray();
        }

        public static MetaBehaviour[] GetMetaBehaviours(this GameObject gameObject, Type type)
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponents<MetaBehaviourRunner>();
            return runners.Where(r => r.Behaviour.GetType() == type).Select(r => r.Behaviour).ToArray();
        }

        public static T[] GetMetaBehavioursInChildren<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponentsInChildren<MetaBehaviourRunner>();
            return runners.Where(r => r.Behaviour.GetType() == typeof(T)).Select(r => (T)r.Behaviour).ToArray();
        }

        public static MetaBehaviour[] GetMetaBehavioursInChildren(this GameObject gameObject, Type type)
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponentsInChildren<MetaBehaviourRunner>();
            return runners.Where(r => r.Behaviour.GetType() == type).Select(r => r.Behaviour).ToArray();
        }

        public static T[] GetMetaBehavioursInParent<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponentsInParent<MetaBehaviourRunner>();
            return runners.Where(r => r.Behaviour.GetType() == typeof(T)).Select(r => (T)r.Behaviour).ToArray();
        }

        public static MetaBehaviour[] GetMetaBehavioursInParent(this GameObject gameObject, Type type)
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponentsInParent<MetaBehaviourRunner>();
            return runners.Where(r => r.Behaviour.GetType() == type).Select(r => r.Behaviour).ToArray();
        }

        public static void Destroy(this IEnumerable<MetaBehaviour> behaviours)
        {
            foreach (MetaBehaviour behaviour in behaviours)
            {
                behaviour.Destroy();
            }
        }
    }
}