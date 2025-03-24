using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class OptionsManagerBase<T> : MonoBehaviour where T : OptionsAssetBase
    {
        [SerializeField] private T optionsAsset;

        protected virtual void Awake()
        {
            optionsAsset.Updated += OnOptionsUpdated;
            OnOptionsUpdated();
        }

        private void OnOptionsUpdated()
        {
            OnOptionsUpdated(optionsAsset);
        }

        protected abstract void OnOptionsUpdated(T optionsAssetT);

        protected virtual void OnDestroy()
        {
            optionsAsset.Updated -= OnOptionsUpdated;
        }
    }
}