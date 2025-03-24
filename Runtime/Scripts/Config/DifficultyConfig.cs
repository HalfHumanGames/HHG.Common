using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class DifficultyConfig
    {
        public IReadOnlyList<DifficultyAssetBase> Difficulties => difficulties;

        [SerializeField] private List<DifficultyAssetBase> difficulties;
    }
}