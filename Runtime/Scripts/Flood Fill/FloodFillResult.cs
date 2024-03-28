using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common
{
    public class FloodFillResult
    {
        public bool AreaBordersEdge;
        public List<Vector3Int> Area;

        public FloodFillResult()
        {
             Area = new List<Vector3Int>();
        }

        public FloodFillResult(bool areaBordersEdge, List<Vector3Int> area)
        {
            AreaBordersEdge = areaBordersEdge;
            Area = area;
        }

        public void Reset()
        {
            AreaBordersEdge = false;
            Area.Clear();
        }
    }
}