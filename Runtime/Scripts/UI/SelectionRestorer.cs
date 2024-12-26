using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class SelectionRestorer : MonoBehaviour
    {
        public Selectable PreviousSelection => previousSelection;

        private HashSet<Selectable> selectables = new HashSet<Selectable>();
        private Selectable previousSelection;

        private void Start()
        {
            // Do in Start in case selectables get created in Awake
            SearchForSelectables();
        }

        private void Update()
        {
            GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;

            if (selectedGameObject == null)
            {
                return;
            }

            if (!selectedGameObject.TryGetComponent(out Selectable selected))
            {
                return;
            }

            if (selected == previousSelection)
            {
                return;
            }

            if (!selectables.Contains(selected))
            {
                return;
            }

            previousSelection = selected;
        }

        public void RestoreSelection()
        {
            if (previousSelection == null)
            {
                // Get first active selectable in children
                previousSelection = GetComponentInChildren<Selectable>();
            }

            if (previousSelection != null)
            {
                previousSelection.Select();
            }
        }

        public void SearchForSelectables()
        {
            selectables.Clear();
            selectables.AddRange(GetComponentsInChildren<Selectable>(true));
        }
    }
}