using System;
using System.Reflection;
using System.Text;

namespace HHG.Common.Runtime
{
    public static class PropertyInfoExtensions
    {
        public static string GetPath(this PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            StringBuilder path = new StringBuilder(property.Name);
            Type type = property.DeclaringType;

            while (type != null)
            {
                path.Insert(0, ".").Insert(0, type.Name);
                type = type.DeclaringType;
            }

            return path.ToString().Trim('.');
        }
    }
}