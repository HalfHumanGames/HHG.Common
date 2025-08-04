using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HHG.Common.Runtime
{
    public static class TypeExtensions
    {
        private static IEnumerable<System.Type> types;
        private static Dictionary<System.Type, IEnumerable<System.Type>> subclasses;

        public static bool IsSubclassOfGeneric(this System.Type type, System.Type generic)
        {
            return type.IsSubclassOfGeneric(generic, out _);
        }

        public static bool IsSubclassOfGeneric(this System.Type type, System.Type generic, out System.Type found)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == generic)
                {
                    found = type;
                    return true;
                }

                type = type.BaseType;
            }

            found = null;
            return false;
        }

        public static List<System.Type> FindSubclasses(this System.Type type, System.Func<System.Type, bool> filter = null)
        {
            filter ??= _ => true;

            if (types == null)
            {
                types = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
            }

            if (subclasses == null)
            {
                subclasses = new Dictionary<System.Type, IEnumerable<System.Type>>();
            }

            if (!subclasses.ContainsKey(type))
            {
                subclasses[type] = types.Where(t => t.IsClass && type.IsAssignableFrom(t));
            }

            return subclasses[type].Where(filter).ToList();
        }

        public static bool Implements(this System.Type type, params System.Type[] interfaces)
        {
            return System.Array.Exists(type.GetInterfaces(), i => interfaces.Contains(i));
        }

        public static bool IsBaseImplementationOf(this System.Type type, System.Type _interface)
        {
            return type == _interface || (type.Implements(_interface) && (type.BaseType == null || !type.BaseType.Implements(_interface)));
        }

        public static bool IsReference(this System.Type type)
        {
            return type.IsClass && type != typeof(string);
        }

        public static bool IsCollection(this System.Type type)
        {
            return type.IsArray ||
                   typeof(ICollection).IsAssignableFrom(type) ||
                   type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

    }
}