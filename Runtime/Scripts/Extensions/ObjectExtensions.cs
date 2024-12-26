using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ObjectExtensions
    {
        public static void SmartDestroy(this Object src, Object obj)
        {
            ObjectUtil.SmartDestroy(obj);
        }

        public static void SmartDestroy(this Object src, IEnumerable<Object> objects)
        {
            ObjectUtil.SmartDestroy(objects);
        }

        public static void Destroy(this Object src, IEnumerable<Object> objects)
        {
            ObjectUtil.Destroy(objects);
        }

        public static void DestroyNextFrame(this Object src, Object obj)
        {
            ObjectUtil.DestroyNextFrame(obj);
        }

        public static void DestroyNextFrame(this Object src, IEnumerable<Object> objects)
        {
            ObjectUtil.DestroyNextFrame(objects);
        }

        // These are similar to the Object.FindObjectOfType methods and its variants
        // but these also work with interfaces while Object.FindObjectOfType methods
        // don't since they require the generic T to be of type UnityEngine.Object

        public static T FindObjectOfType<T>(this Object obj, bool includeInactive = false)
        {
            return ObjectUtil.FindObjectOfType<T>(includeInactive); ;
        }

        public static T[] FindObjectsOfType<T>(this Object obj, bool includeInactive = false)
        {
            return ObjectUtil.FindObjectsOfType<T>(includeInactive);
        }

        public static T CloneFromJson<T>(this T obj)
        {
            // Use this instead of the generic version since it also works in
            // cases when T is either an abstract class or an interface
            return (T)JsonUtility.FromJson(JsonUtility.ToJson(obj), obj.GetType());
        }

        public static List<T> CloneFromJson<T>(this IEnumerable<T> objs)
        {
            List<T> list = new List<T>();

            foreach (T obj in objs)
            {
                list.Add(obj.CloneFromJson());
            }

            return list;
        }
    }
}