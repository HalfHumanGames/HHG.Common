using UnityEngine;

namespace HHG.Common.Runtime
{
    public class FloodFillSearchResult : FloodFillResultBase
    {
        public bool IsSuccess;
        public Vector3Int TargetPosition;

        public override void Reset()
        {
            IsSuccess = false;
            TargetPosition = default;
        }
    }
}