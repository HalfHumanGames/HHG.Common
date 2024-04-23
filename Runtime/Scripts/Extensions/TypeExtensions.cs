using System;
using System.Collections.Generic;
using System.Linq;

namespace HHG.Common.Runtime
{
    public static class TypeExtensions
    {
        public static List<Type> FindSubclasses(this Type type, Func<Type, bool> filter = null)
        {
            filter ??= _ => true;
            return AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(assembly => assembly.GetTypes()).
                Where(t => t.IsClass && type.IsAssignableFrom(t) && t != type && filter(t)). 
                ToList();
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