using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        public TMP_Text Label => label.FromComponentInChildren(this, true);
        public Button Button => button.FromComponent(this);
        public EventTrigger EventTrigger => eventTrigger.FromComponent(this);
        public ActionEvent<UIButton> OnClick => onClick;

        [SerializeField] private bool singleUse;
        [SerializeField] private ActionEvent<UIButton> onClick = new ActionEvent<UIButton>();

        private Lazy<TMP_Text> label = new Lazy<TMP_Text>();
        private Lazy<Button> button = new Lazy<Button>();
        private Lazy<EventTrigger> eventTrigger = new Lazy<EventTrigger>();

        private void Awake()
        {
            Button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if (singleUse)
            {
                Button.interactable = false;
            }

            onClick.Invoke(this);
        }
    }
}
