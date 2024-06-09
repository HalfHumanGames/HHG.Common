using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class FloodFillSearchResult : FloodFillResultBase
    {
        public bool IsSuccess;
        public Vector3Int TargetPosition;
        public float Distance;
        public List<Vector3Int> Path = new List<Vector3Int>();

        public override void Reset()
        {
            IsSuccess = false;
            TargetPosition = default;
            Distance = 0;
            Path.Clear();
        }
    }
}