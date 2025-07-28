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

            throw new System.ArgumentException($"Unable to get value by path '{path}'");
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

                System.Type type = obj.GetType();

                obj = type.GetField(part, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj) ??
                      type.GetProperty(part, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj);

                if (obj == null)
                {
                    return false;
                }
            }

            try
            {
                value = obj is T t ? t : (T)System.Convert.ChangeType(obj, typeof(T));
                return true;
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                return false;
            }
        }

        public static void SetValueByPath(this object obj, string path, object value)
        {
            if (!TrySetValueByPath(obj, path, value))
            {
                throw new System.ArgumentException($"Unable to set value by path '{path}'");
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

                System.Type type = obj.GetType();

                obj = type.GetField(parts[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj) ??
                      type.GetProperty(parts[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj);

                if (obj == null)
                {
                    return false;
                }
            }

            string lastPart = parts[^1];
            System.Type lastType = obj.GetType();
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

        public static void FromMemberwiseOverwrite(this object obj, object source, OverwriteOptions options = OverwriteOptions.Default)
        {
            if (obj == null || source == null) return;

            bool excludeFields = options.HasFlag(OverwriteOptions.ExcludeFields);
            bool excludeProperties = options.HasFlag(OverwriteOptions.ExcludeProperties);
            bool excludeNonPublic = options.HasFlag(OverwriteOptions.ExcludeNonPublic);
            bool excludePublic = options.HasFlag(OverwriteOptions.ExcludePublic);
            bool excludeReferences = options.HasFlag(OverwriteOptions.ExcludeReferences);
            bool excludeCollections = options.HasFlag(OverwriteOptions.ExcludeCollections);

            System.Type type = obj.GetType();

            BindingFlags flags = BindingFlags.Instance;
            if (!excludePublic) flags |= BindingFlags.Public;
            if (!excludeNonPublic) flags |= BindingFlags.NonPublic;

            while (type != null)
            {
                if (!excludeFields)
                {
                    foreach (FieldInfo field in type.GetFields(flags))
                    {
                        if (field.IsInitOnly) continue;

                        System.Type fieldType = field.FieldType;
                        if (excludeReferences && fieldType.IsReference()) continue;
                        if (excludeCollections && fieldType.IsCollection()) continue;

                        field.SetValue(obj, field.GetValue(source));
                    }
                }

                if (!excludeProperties)
                {
                    foreach (PropertyInfo prop in type.GetProperties(flags))
                    {
                        if (!prop.CanWrite) continue;

                        System.Type propType = prop.PropertyType;
                        if (excludeReferences && propType.IsReference()) continue;
                        if (excludeCollections && propType.IsCollection()) continue;

                        prop.SetValue(obj, prop.GetValue(source));
                    }
                }

                type = type.BaseType;
            }
        }
    }
}
