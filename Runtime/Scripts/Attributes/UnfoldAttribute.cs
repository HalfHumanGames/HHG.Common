using UnityEngine;

namespace HHG.Common.Runtime
{
    public enum UnfoldName
    {
        Auto, // Not implemented yet
        Full,
        Child,
        Parent
    }

    public class UnfoldAttribute : PropertyAttribute
    {
        public UnfoldName Name => name;

        private UnfoldName name;

        public UnfoldAttribute(UnfoldName unfoldName = UnfoldName.Auto)
        {
            name = unfoldName;   
        }
    }
}