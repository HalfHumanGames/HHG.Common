using System.Linq;
using System;
using System.Reflection;

namespace HHG.Common.Runtime
{
    public static class TypeUtil
    {
        public static Type FindType(string className)
        {
            // Get all loaded assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Search through each assembly for the type
            foreach (Assembly assembly in assemblies)
            {
                // Get all types in the assembly
                Type[] types = assembly.GetTypes();

                // Search for the type by name
                Type type = types.FirstOrDefault(t => t.Name.Equals(className));

                if (type != null)
                {
                    return type; // Found the type
                }
            }

            return null; // Type not found
        }
    }
}