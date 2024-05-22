using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    public static class ObjectUtil
    {
        public static void Destroy(this IEnumerable<Object> objects)
        {
            if (objects != null)
            {
                foreach (Object obj in objects.Distinct())
                {
                    Object.Destroy(obj);
                }
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