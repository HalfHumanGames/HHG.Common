using System;

namespace HHG.Common.Runtime
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class OptionsCategoryAttribute : Attribute
    {
        public string Category => category;

        private string category;

        public OptionsCategoryAttribute(string category)
        {
            this.category = category;
        }
    }
}