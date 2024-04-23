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
        public Func<object, bool> filter;

        public DropdownAttribute(Type typeFilter = null, string getter = null)
        {
            type = typeFilter;
            filter = !string.IsNullOrEmpty(getter) && type.GetProperty(getter)?.GetGetMethod() is MethodInfo method ? obj => (bool)method.Invoke(obj, null) : _ => true;
        }
    }
}