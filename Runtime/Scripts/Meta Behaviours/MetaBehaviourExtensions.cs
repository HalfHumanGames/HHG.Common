using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class MetaBehaviourExtensions
    {
        public static T GetOrAddMetaBehaviour<T>(this GameObject gameObject, T metaBehaviour) where T : MetaBehaviour
        {
            return gameObject.GetMetaBehaviour<T>() ?? gameObject.AddMetaBehaviour(metaBehaviour);
        }

        public static T AddMetaBehaviour<T>(this GameObject gameObject, T metaBehaviour) where T : MetaBehaviour
        {
            MetaBehaviourRunner runner = gameObject.AddComponent<MetaBehaviourRunner>();
            if (gameObject.activeInHierarchy)
            {
                runner.enabled = false;
            }
            T behaviour = (T)runner.SetBehaviour(metaBehaviour.Clone());
            if (gameObject.activeInHierarchy)
            {
                behaviour.Awake();
                behaviour.enabled = true;
            }
            return behaviour;
        }

        public static T[] AddMetaBehaviours<T>(this GameObject gameObject, IEnumerable<T> metaBehaviours) where T : MetaBehaviour
        {
            int count = metaBehaviours.Count();
            T[] behaviours = new T[count];

            int i = 0;
            foreach (T metaBehaviour in metaBehaviours)
            {
                MetaBehaviourRunner runner = gameObject.AddComponent<MetaBehaviourRunner>();
                if (gameObject.activeInHierarchy)
                {
                    runner.enabled = false;
                }
                behaviours[i++] = (T)runner.SetBehaviour(metaBehaviour.Clone());
            }

            if (gameObject.activeInHierarchy)
            {
                foreach (T behaviour in behaviours)
                {
                    behaviour.Awake();
                    behaviour.enabled = true;
                }
            }

            return behaviours;
        }

        public static T GetMetaBehaviour<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            return (T) gameObject.GetMetaBehaviour(typeof(T));
        }

        public static MetaBehaviour GetMetaBehaviour(this GameObject gameObject, Type type) 
        {
            return gameObject.GetComponents<MetaBehaviourRunner>().FirstOrDefault(r => type.IsAssignableFrom(r.Behaviour.GetType()))?.Behaviour;
        }

        public static T GetMetaBehaviourInChildren<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            return (T)gameObject.GetMetaBehaviourInChildren(typeof(T));
        }

        public static MetaBehaviour GetMetaBehaviourInChildren(this GameObject gameObject, Type type)
        {
            return gameObject.GetComponentsInChildren<MetaBehaviourRunner>().FirstOrDefault(r => type.IsAssignableFrom(r.Behaviour.GetType()))?.Behaviour;
        }

        public static T GetMetaBehaviourInParent<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            return (T)gameObject.GetMetaBehaviourInParent(typeof(T));
        }

        public static MetaBehaviour GetMetaBehaviourInParent(this GameObject gameObject, Type type)
        {
            return gameObject.GetComponentsInParent<MetaBehaviourRunner>().FirstOrDefault(r => type.IsAssignableFrom(r.Behaviour.GetType()))?.Behaviour;
        }

        public static T[] GetMetaBehaviours<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponents<MetaBehaviourRunner>();
            return runners.Where(r => typeof(T).IsAssignableFrom(r.Behaviour.GetType())).Select(r => (T)r.Behaviour).ToArray();
        }

        public static MetaBehaviour[] GetMetaBehaviours(this GameObject gameObject, Type type)
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponents<MetaBehaviourRunner>();
            return runners.Where(r => type.IsAssignableFrom(r.Behaviour.GetType())).Select(r => r.Behaviour).ToArray();
        }

        public static T[] GetMetaBehavioursInChildren<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponentsInChildren<MetaBehaviourRunner>();
            return runners.Where(r => typeof(T).IsAssignableFrom(r.Behaviour.GetType())).Select(r => (T)r.Behaviour).ToArray();
        }

        public static MetaBehaviour[] GetMetaBehavioursInChildren(this GameObject gameObject, Type type)
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponentsInChildren<MetaBehaviourRunner>();
            return runners.Where(r => type.IsAssignableFrom(r.Behaviour.GetType())).Select(r => r.Behaviour).ToArray();
        }

        public static T[] GetMetaBehavioursInParent<T>(this GameObject gameObject) where T : MetaBehaviour
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponentsInParent<MetaBehaviourRunner>();
            return runners.Where(r => typeof(T).IsAssignableFrom(r.Behaviour.GetType())).Select(r => (T)r.Behaviour).ToArray();
        }

        public static MetaBehaviour[] GetMetaBehavioursInParent(this GameObject gameObject, Type type)
        {
            MetaBehaviourRunner[] runners = gameObject.GetComponentsInParent<MetaBehaviourRunner>();
            return runners.Where(r => type.IsAssignableFrom(r.Behaviour.GetType())).Select(r => r.Behaviour).ToArray();
        }

        public static void Destroy(this IEnumerable<MetaBehaviour> behaviours, float delay = 0f)
        {
            foreach (MetaBehaviour behaviour in behaviours)
            {
                behaviour.Destroy(delay);
            }
        }

        public static void SmartDestroy(this IEnumerable<MetaBehaviour> behaviours)
        {
            foreach (MetaBehaviour behaviour in behaviours)
            {
                behaviour.SmartDestroy();
            }
        }
    }
}