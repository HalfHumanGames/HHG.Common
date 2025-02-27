using System;
using System.Reflection;

namespace HHG.Common.Runtime
{
    public static class ReflectionExtensions
    {
        public static T GetValueByPath<T>(this object obj, string path)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            }

            foreach (string part in path.Split('.'))
            {
                if (obj == null)
                {
                    throw new NullReferenceException($"Null encountered while accessing '{part}'");
                }

                Type type = obj.GetType();

                obj = type.GetField(part, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj) ??
                      type.GetProperty(part, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj);

                if (obj == null)
                {
                    throw new ArgumentException($"Field or property '{part}' not found in '{type.FullName}'");
                }
            }
            return obj is T t ? t : default;
        }

        public static void SetValueByPath(this object obj, string path, object value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            }

            string[] parts = path.Split('.');

            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (obj == null)
                {
                    throw new NullReferenceException($"Null encountered while accessing '{parts[i]}'");
                }

                Type type = obj.GetType();

                obj = type.GetField(parts[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj) ??
                      type.GetProperty(parts[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj);

                if (obj == null)
                {
                    throw new ArgumentException($"Field or property '{parts[i]}' not found in '{type.FullName}'");
                }
            }

            string lastPart = parts[^1];
            Type lastType = obj.GetType();
            FieldInfo field = lastType.GetField(lastPart, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo property = lastType.GetProperty(lastPart, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else if (property?.CanWrite == true)
            {
                property.SetValue(obj, value);
            }
            else
            {
                throw new ArgumentException($"Field or property '{lastPart}' not found or not writable in '{lastType.FullName}'");
            }
        }
    }
}