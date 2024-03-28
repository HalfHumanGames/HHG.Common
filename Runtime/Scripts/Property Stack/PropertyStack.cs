using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [Serializable]
    public class PropertyStack<T>
    {
        public T Value => stack.Count == 0 ? default : stack[stack.Count - 1].Value;

        [SerializeField] private T value;

        private List<StackItem> stack = new List<StackItem>();

        private struct StackItem
        {
            public bool IsValid => Source != null;
            public object Source;
            public T Value;

            public StackItem(object source, T value)
            {
                Source = source;
                Value = value;
            }
        }

        public PropertyStack()
        {
            Push(this, value);
        }

        public PropertyStack(T initial)
        {
            value = initial;
            Push(this, value);
        }

        public void Push(object source, T value)
        {
            stack.Add(new StackItem(source, value));
        }

        public void Pop(object source)
        {
            StackItem remove = stack.LastOrDefault(s => s.Source == source);

            if (remove.IsValid)
            {
                stack.Remove(remove);
            }
        }

        public void Clear()
        {
            stack.Clear();
            Push(this, value);
        }

        public static implicit operator T(PropertyStack<T> propertyStack)
        {
            return propertyStack.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}