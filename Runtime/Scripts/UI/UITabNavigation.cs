using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class UITabNavigation : MonoBehaviour
    {
        [SerializeField] private Transform tabsContainer;
        [SerializeField] private Transform contentContainer;

        private Lazy<Button> _tabs = new Lazy<Button>();
        private Button[] tabs => _tabs.FromComponentsInChildren(tabsContainer);
        private Lazy<GameObject> _content = new Lazy<GameObject>();
        private GameObject[] content => _content.From(() => contentContainer.GetChildren().Select(t => t.gameObject).ToArray());

        private void Awake()
        {
            foreach (Button tab in tabs)
            {
                tab.onClick.AddListener(() => SelectTab(tab));
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

            // Makes sure the horizontal/vertical layout groups get rebuild which may not happen by default
            LayoutRebuilder.ForceRebuildLayoutImmediate(content[index].GetComponent<RectTransform>());
        }
    }
}