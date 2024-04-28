using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class ObjectUtil
    {
        public static void Destroy(this IEnumerable<Object> objects)
        {
            if (objects != null)
            {
                foreach (Object obj in objects.Distinct())
                {
                    Object.Destroy(obj);
                }
            }
        }
    }
}