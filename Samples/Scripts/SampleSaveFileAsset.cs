using HHG.Common.Runtime;
using UnityEngine;

namespace HHG.Common.Sample
{
    [CreateAssetMenu(fileName = "Sample Save File", menuName = "HHG/Sample/Sample Save File", order = 0)]
    public class SampleSaveFileAsset : SaveFileAssetBase
    {
        public int Gold { get => Get(ref gold); set => Set(ref gold, value); }

        [SerializeField] private int gold = 100;

        public override void Reset()
        {
            Gold = 0;
        }
    }
}