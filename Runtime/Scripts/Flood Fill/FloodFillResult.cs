using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class FloodFillResult
    {
        public bool AreaBordersEdge;
        public HashSet<Vector3Int> Area = new HashSet<Vector3Int>();

        public void Reset()
        {
            AreaBordersEdge = false;
            Area.Clear();
        }
    }
}