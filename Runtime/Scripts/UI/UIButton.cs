using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        public TMP_Text Label => label;
        public Button Button => button;
        public EventTrigger EventTrigger => eventTrigger;
        public ActionEvent<UIButton> OnClick => onClick;

        [SerializeField] private bool singleUse;
        [SerializeField] private ActionEvent<UIButton> onClick = new ActionEvent<UIButton>();

        private TMP_Text label;
        private Button button;
        private EventTrigger eventTrigger;

        private void Awake()
        {
            label = GetComponentInChildren<TMP_Text>(true);
            button = GetComponent<Button>();
            eventTrigger = GetComponent<EventTrigger>();

            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            onClick.Invoke(this);

            if (singleUse)
            {
                button.interactable = false;
            }
        }
    }
}
