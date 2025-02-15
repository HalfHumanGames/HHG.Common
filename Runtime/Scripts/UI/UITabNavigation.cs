using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class UITabNavigation : MonoBehaviour
    {
        [SerializeField] private Transform tabsContainer;
        [SerializeField] private Transform contentContainer;

        private RectTransform rectTransform;
        private Lazy<Button> _tabs = new Lazy<Button>();
        private Button[] tabs => _tabs.FromComponentsInChildren(tabsContainer);
        private Lazy<GameObject> _content = new Lazy<GameObject>();
        private GameObject[] content => _content.From(() => contentContainer.GetChildren().Select(t => t.gameObject).ToArray());

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            foreach (Button tab in tabs)
            {
                tab.onClick.AddListener(() => SelectTab(tab));

                if (tab.TryGetComponent(out EventTrigger eventTrigger))
                {
                    eventTrigger.AddTrigger(EventTriggerType.Select, () => SelectTab(tab));
                }
            }

            tabs.SetNavigationHorizontal();

            int len = Mathf.Min(tabs.Length, content.Length);
            for (int i = 0; i < len; i++)
            {
                Button tab = tabs[i];
                GameObject item = content[i];

                // Make sure to only get the topmost selectable in each child game object
                // since certain ui elements like dropdowns contain child selectables
                // Also make sure to call NotNull in case a tab section has no selectables
                var selectables = item.transform.GetChildren().Select(c => c.GetComponentInChildren<Selectable>(true)).NotNull();
                selectables.SetNavigationVertical();
                
                if (selectables.Any())
                {
                    Selectable first = selectables.First();
                    first.SetNavigationUp(tab);
                    tab.SetNavigationDown(first);
                }
            }
        }

        private void OnEnable()
        {
            SelectTab(tabs.FirstOrDefault());
        }

        public void SelectTab(Button button)
        {
            foreach (GameObject item in content)
            {
                item.SetActive(false);
            }

            int index = button.transform.GetSiblingIndex();
            content[index].SetActive(true);
            rectTransform.RebuildLayout();
        }
    }
}