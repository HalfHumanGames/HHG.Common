using TMPro;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class UIBuildVersion : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        private void Start()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (label == null)
            {
                label = GetComponent<TextMeshProUGUI>();
            }

            if (label != null)
            {
                label.text = Application.version;
            }
        }

        private void OnValidate()
        {
            Refresh();
        }
    }
}
