using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    // Ordered list of sub-assets stored inside the owning asset.
    // Edit it through the Sub-Asset tools (inspector Add button, Project window right-click), not by hand.
    [System.Serializable]
    public class SubAssetList<T> where T : ScriptableObject
    {
        public IReadOnlyList<T> Items => items;

        [SerializeField] private List<T> items = new List<T>();
    }
}
