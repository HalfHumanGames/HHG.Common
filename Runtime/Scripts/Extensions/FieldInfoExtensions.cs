using System;
using System.Reflection;
using System.Text;

namespace HHG.Common.Runtime
{
    public static class FieldInfoExtensions
    {
        public static string GetPath(this FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            StringBuilder path = new StringBuilder(field.Name);
            Type type = field.DeclaringType;

            while (type != null)
            {
                path.Insert(0, ".").Insert(0, type.Name);
                type = type.DeclaringType;
            }

            return path.ToString().Trim('.');
        }
    }
}