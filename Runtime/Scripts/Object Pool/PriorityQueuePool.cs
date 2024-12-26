using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace HHG.Common.Runtime
{
    public class PriorityQueuePool<T, TPriority> : CollectionPool<PriorityQueue<T, TPriority>, KeyValuePair<T, TPriority>> where TPriority : IComparable<TPriority>
    {

    }
}