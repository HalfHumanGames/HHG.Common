using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class DifficultyManagerBase<TSaveFileAsset, TDifficultyAsset> : MonoBehaviour 
        where TSaveFileAsset : SaveFileAssetBase
        where TDifficultyAsset : DifficultyAssetBase
    {
        [SerializeField] private TSaveFileAsset saveFileAsset;

        private int difficultyIndex = -1;

        private void Awake()
        {
            saveFileAsset.Updated += OnSaveFileUpdated;
            OnSaveFileUpdated();
        }

        private void OnSaveFileUpdated()
        {
            if (difficultyIndex != saveFileAsset.DifficultyIndex)
            {
                difficultyIndex = saveFileAsset.DifficultyIndex;

                TDifficultyAsset difficultyAsset = ConfigBase.Difficulty.Difficulties[difficultyIndex] as TDifficultyAsset;

                OnDifficultyUpdated(difficultyAsset);
            }
        }

        protected abstract void OnDifficultyUpdated(TDifficultyAsset difficultyAsset);

        private void OnDestroy()
        {
            saveFileAsset.Updated -= OnSaveFileUpdated;
        }
    }
}