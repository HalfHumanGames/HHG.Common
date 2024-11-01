using System;
using System.Reflection;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class DropdownAttribute : PropertyAttribute
    {
        public Type Type => type;
        public Func<object, bool> Filter => filter;

        private Type type;
        private Func<object, bool> filter;

        public DropdownAttribute()
        {

        }

        public DropdownAttribute(Type typeFilter)
        {
            type = typeFilter;
        }

        public DropdownAttribute(Type typeFilter, string getter) : this(typeFilter)
        {
            if (!string.IsNullOrEmpty(getter))
            {
                PropertyInfo property = type.GetProperty(getter);

                if (property != null && property.GetGetMethod() is MethodInfo method)
                {
                    filter = obj => (bool)method.Invoke(obj, null);
                }
            }
            else
            {
                filter = _ => true;
            }
        }
    }
}