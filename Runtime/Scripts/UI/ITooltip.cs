using UnityEngine;

namespace HHG.Common.Runtime
{
    public interface ITooltip
    {
        public string TooltipText { get; }
        public Vector3 TooltipPosition => this is Component component ? component.transform.position : default;
    }
}