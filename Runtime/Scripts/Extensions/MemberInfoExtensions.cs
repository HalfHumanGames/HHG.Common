using System;
using System.Reflection;
using System.Text;

namespace HHG.Common.Runtime
{
    public static class MemberInfoExtensions
    {
        public static bool HasCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));

            }
            return member.GetCustomAttribute<T>() != null;
        }

        public static bool TryGetCustomAttribute<T>(this MemberInfo member, out T attribte) where T : Attribute
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            attribte = member.GetCustomAttribute<T>();
            return attribte != null;
        }

        public static string GetPath(this MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            StringBuilder path = new StringBuilder(member.Name);
            Type type = member.DeclaringType;

            while (type != null)
            {
                path.Insert(0, ".").Insert(0, type.Name);
                type = type.DeclaringType;
            }

            return path.ToString().Trim('.');
        }
    }
}