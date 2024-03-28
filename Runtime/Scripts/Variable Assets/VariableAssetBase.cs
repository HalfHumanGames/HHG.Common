using UnityEngine;

namespace HHG.Common
{
    public abstract class VariableAssetBase : ScriptableObject
    {
        public abstract object WeakValue { get; set; }
    }

    public abstract class VariableAssetBase<T> : VariableAssetBase
    {
        public override object WeakValue
        {
            get => value;
            set => this.value = (T)value;
        }

        public T Value
        {
            get => value;
            set => this.value = value;
        }

        [SerializeField] protected T value;
    }
}