using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class SelectableNavigationSetter : MonoBehaviour
    {
        [SerializeField] private SelectableNavigation mode;

        private List<Selectable> selectables = new List<Selectable>();

        private void OnEnable()
        {
            GetComponentsInChildren(selectables);

            selectables.Where(s => s.isActiveAndEnabled && s.interactable).SetNavigation(mode);
        }
    }
}