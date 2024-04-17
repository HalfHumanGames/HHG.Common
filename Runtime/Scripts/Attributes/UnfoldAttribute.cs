using UnityEngine;

namespace HHG.Common.Runtime
{
    public enum UnfoldName
    {
        Auto,
        Full,
        Child,
        Parent
    }

    public class UnfoldAttribute : PropertyAttribute
    {
        public UnfoldName Name => name;

        private UnfoldName name;

        public UnfoldAttribute(UnfoldName unfoldName = UnfoldName.Full)
        {
            name = unfoldName;   
        }
    }
}