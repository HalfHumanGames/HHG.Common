using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class LayerMaskUtility
    {
        // LayerMask.GetMask only accepts an array, so this prevents
        // having to convert a list or any enumerable to an array
        public static int GetMask(IEnumerable<string> layerNames)
        {
            if (layerNames == null)
            {
                throw new ArgumentNullException("layerNames");
            }

            int layerMask = 0;
            foreach (string layerName in layerNames)
            {
                int layer = LayerMask.NameToLayer(layerName);
                if (layer != -1)
                {
                    layerMask |= 1 << layer;
                }
            }

            return layerMask;
        }
    }
}