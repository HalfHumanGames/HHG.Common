using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        public Button Button => button;
        public ActionEvent OnClick => onClick;

        [SerializeField] private bool singleUse;
        [SerializeField] private ActionEvent onClick = new ActionEvent();

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
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
