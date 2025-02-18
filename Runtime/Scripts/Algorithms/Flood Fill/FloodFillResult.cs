using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class FloodFillResult : FloodFillResultBase
    {
        public bool AreaBordersEdge;
        public List<Vector3Int> Area = new List<Vector3Int>();

        public override void Reset()
        {
            AreaBordersEdge = false;
            Area.Clear();
        }
    }
}