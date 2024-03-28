using System;
using UnityEngine;

namespace HHG.Common
{
    public class FilterAttribute : PropertyAttribute
    {
        public Type Type => type;

        private Type type;

        public FilterAttribute(Type type)
        {
            this.type = type;
        }
    }
}