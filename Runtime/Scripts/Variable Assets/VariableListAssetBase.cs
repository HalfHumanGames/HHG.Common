using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace HHG.Common.Runtime
{
    public abstract class VariableListAssetBase : ScriptableObject
    {
        public abstract IReadOnlyList<object> WeakList { get; }
    }

    public abstract class VariableListAssetBase<T> : VariableListAssetBase
    {
        public override IReadOnlyList<object> WeakList
        {
            get
            {
                if (weakList == null)
                {
                    weakList = new List<object>(list.Cast<object>());
                }

                return weakList;
            }
        }

        public IReadOnlyList<T> List => list;

        [SerializeField, FormerlySerializedAs("value")] protected List<T> list = new List<T>();

        private List<object> weakList;
    }
}