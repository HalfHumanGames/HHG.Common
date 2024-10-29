using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class VariableListAssetBase : ScriptableObject
    {
        public abstract IReadOnlyList<object> WeakValue { get; }
    }

    public abstract class VariableListAssetBase<T> : VariableListAssetBase
    {
        public override IReadOnlyList<object> WeakValue
        {
            get
            {
                if (weakValue == null)
                {
                    weakValue = new List<object>(value.Cast<object>());
                }

                return weakValue;
            }
        }

        public IReadOnlyList<T> Value => value;

        [SerializeField] protected List<T> value = new List<T>();

        private List<object> weakValue;
    }
}