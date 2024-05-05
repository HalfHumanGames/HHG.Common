using System;
using System.Collections.Generic;
using System.Linq;

namespace HHG.Common.Runtime
{
    public static class TypeExtensions
    {
        private static IEnumerable<Type> types;
        private static Dictionary<Type, IEnumerable<Type>> subclasses;

        public static List<Type> FindSubclasses(this Type type, Func<Type, bool> filter = null)
        {
            filter ??= _ => true;

            if (types == null)
            {
                types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
            }

            if (subclasses == null)
            {
                subclasses = new Dictionary<Type, IEnumerable<Type>>();
            }

            if (!subclasses.ContainsKey(type))
            {
                subclasses[type] = types.Where(t => t.IsClass && type.IsAssignableFrom(t));
            }

            return subclasses[type].Where(filter).ToList();
        }

        public static bool Implements(this Type type, params Type[] interfaces)
        {
            return Array.Exists(type.GetInterfaces(), i => interfaces.Contains(i));
        }

        public static bool IsBaseImplementationOf(this Type type, Type _interface)
        {
            return type == _interface || (type.Implements(_interface) && (type.BaseType == null || !type.BaseType.Implements(_interface)));
        }
    }
}