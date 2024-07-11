using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    public static class ObjectUtil
    {
        public static void SmartDestroy(Object obj)
        {
            if (obj != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(obj);
                }
                else
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }

        public static void SmartDestroy(IEnumerable<Object> objects)
        {
            if (objects != null)
            {
                foreach (Object obj in objects.Distinct())
                {
                    SmartDestroy(obj);
                }
            }
        }

        public static void Destroy(IEnumerable<Object> objects)
        {
            if (objects != null)
            {
                foreach (Object obj in objects.Distinct())
                {
                    Object.Destroy(obj);
                }
            }
        }

        public static void DestroyNextFrame(Object obj)
        {
            if (obj != null && CoroutineUtil.Coroutiner != null)
            {
                CoroutineUtil.Coroutiner.Invoker().NextFrame(_ => Object.Destroy(obj));
            }
        }

        public static void DestroyNextFrame(IEnumerable<Object> objects)
        {
            if (objects != null && CoroutineUtil.Coroutiner != null)
            {
                CoroutineUtil.Coroutiner.Invoker().NextFrame(_ => Destroy(objects));
            }
        }

        // These are similar to the Object.FindObjectOfType methods and its variants
        // but these also work with interfaces while Object.FindObjectOfType methods
        // don't since they require the generic T to be of type UnityEngine.Object

        public static T FindObjectOfType<T>(bool includeInactive = false)
        {
            T retval = default;
            SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(g => g.TryGetComponentInChildren(out retval, includeInactive));
            return retval;
        }

        public static T[] FindObjectsOfType<T>(bool includeInactive = false)
        {
            return SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<T>(includeInactive)).ToArray();
        }
    }
}