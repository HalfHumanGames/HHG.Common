using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public struct LinkNavigation
    {
        public bool IsEmpty => 
            Up == null || 
            Down == null || 
            Left == null || 
            Right == null;

        public Selectable Up;
        public Selectable Down;
        public Selectable Left;
        public Selectable Right;

        public bool Contains(GameObject target) => 
            Up != null && Up.gameObject == target ||
            Down != null && Down.gameObject == target == target ||
            Left != null && Left.gameObject == target == target ||
            Right != null && Right.gameObject == target == target;
    }
}