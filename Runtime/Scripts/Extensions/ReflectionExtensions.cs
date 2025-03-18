using System;
using System.Reflection;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ReflectionExtensions
    {
        public static object GetValueByPath(this object obj, string path)
        {
            return GetValueByPath<object>(obj, path);
        }

        public static T GetValueByPath<T>(this object obj, string path)
        {
            if (TryGetValueByPath(obj, path, out T value))
            {
                return value;
            }

            throw new ArgumentException($"Unable to get value by path '{path}'");
        }

        public static bool TryGetValueByPath(this object obj, string path, out object value)
        {
            return TryGetValueByPath<object>(obj, path, out value);
        }

        public static bool TryGetValueByPath<T>(this object obj, string path, out T value)
        {
            value = default;

            if (obj == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            foreach (string part in path.Split('.'))
            {
                if (obj == null)
                {
                    return false;
                }

                Type type = obj.GetType();

                obj = type.GetField(part, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj) ??
                      type.GetProperty(part, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj);

                if (obj == null)
                {
                    return false;
                }
            }

            try
            {
                value = obj is T t ? t : (T)Convert.ChangeType(obj, typeof(T));
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return false;
            }
        }

        public static void SetValueByPath(this object obj, string path, object value)
        {
            if (!TrySetValueByPath(obj, path, value))
            {
                throw new ArgumentException($"Unable to set value by path '{path}'");
            }
        }

        public static bool TrySetValueByPath(this object obj, string path, object value)
        {
            if (obj == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            string[] parts = path.Split('.');

            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (obj == null)
                {
                    return false;
                }

                Type type = obj.GetType();

                obj = type.GetField(parts[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj) ??
                      type.GetProperty(parts[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj);

                if (obj == null)
                {
                    return false;
                }
            }

            string lastPart = parts[^1];
            Type lastType = obj.GetType();
            FieldInfo field = lastType.GetField(lastPart, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo property = lastType.GetProperty(lastPart, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field != null)
            {
                field.SetValue(obj, value);
                return true;
            }
            else if (property?.CanWrite == true)
            {
                property.SetValue(obj, value);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
