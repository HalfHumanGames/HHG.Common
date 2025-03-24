using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class DifficultyAssetBase : ScriptableObject
    {
        public string DisplayName => displayName;
        public string Description => description;

        [SerializeField] private string displayName;
        [SerializeField] private string description;
    }
}