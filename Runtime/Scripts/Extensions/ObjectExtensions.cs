using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common
{
    public static class ObjectExtensions
    {
        public static T FindObjectOfType<T>(this Object obj, bool includeInactive = false)
        {
            T retval = default;
            SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(g => g.TryGetComponentInChildren(out retval, includeInactive));
            return retval;
        }

        public static T[] FindObjectsOfType<T>(this Object obj, bool includeInactive = false)
        {
            return SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<T>(includeInactive)).ToArray();
        }
    }
}