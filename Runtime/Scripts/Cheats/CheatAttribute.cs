using System;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CheatAttribute : Attribute
    {
        public Key Key { get; }
        public string Description { get; }

        public CheatAttribute(Key key, string description = "")
        {
            Key = key;
            Description = description;
        }
    }
}