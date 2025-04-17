using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class UITabNavigation : MonoBehaviour
    {
        public Transform TabContainer => tabContainer;
        public Transform ContentContainer => contentContainer;
        public UIButton[] Tabs => tabs;
        public GameObject[] Contents => contents;

        [SerializeField] private Transform tabContainer;
        [SerializeField] private Transform contentContainer;

        private RectTransform rectTransform;
        private Lazy<UIButton> _tabs = new Lazy<UIButton>();
        private UIButton[] tabs => _tabs.FromComponentsInChildren(tabContainer, true);
        private Lazy<GameObject> _contents = new Lazy<GameObject>();
        private GameObject[] contents => _contents.From(() => contentContainer.GetChildren().Select(t => t.gameObject).ToArray());

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _tabs.Reset();
            _contents.Reset();

            rectTransform = GetComponent<RectTransform>();

            foreach (UIButton tab in tabs)
            {
                tab.OnClick.RemoveListener(SelectTab);
                tab.OnClick.AddListener(SelectTab);

                if (tab.TryGetComponent(out EventTrigger eventTrigger))
                {
                    eventTrigger.RemoveTrigger(EventTriggerType.Select, SelectTab);
                    eventTrigger.AddTrigger(EventTriggerType.Select, SelectTab);
                }
            }

            tabs.Select(t => t.Button).SetNavigationHorizontal();

            int len = Mathf.Min(tabs.Length, contents.Length);
            for (int i = 0; i < len; i++)
            {
                UIButton tab = tabs[i];
                GameObject item = contents[i];

                // Make sure to only get the topmost selectable in each child game object
                // since certain ui elements like dropdowns contain child selectables
                // Also make sure to call NotNull in case a tab section has no selectables
                var selectables = item.transform.GetChildren().Select(c => c.GetComponentInChildren<Selectable>(true)).NotNull();
                selectables.SetNavigationVertical();

                if (selectables.Any())
                {
                    Selectable first = selectables.First();
                    first.SetNavigationUp(tab.Button);
                    tab.Button.SetNavigationDown(first);
                }

                if (i == 0)
                {
                    SelectTab(tab);
                }
            }
        }

        private void SelectTab(UIButton button)
        {
            foreach (GameObject item in contents)
            {
                if (item)
                {
                    item.SetActive(false);
                }
            }

            int index = button.transform.GetSiblingIndex();

            if (index < contents.Length)
            {
                if (contents[index] is GameObject content)
                {
                    content.SetActive(true);
                }
            }

            rectTransform.RebuildLayout();
        }

        private void SelectTab(BaseEventData data)
        {
            if (data.selectedObject.TryGetComponent(out UIButton button))
            {
                SelectTab(button);
            }
        }
    }
}