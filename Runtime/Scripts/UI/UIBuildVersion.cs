using TMPro;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class UIBuildVersion : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        private void Start()
        {
            label.text = Application.version;
        }
    }
}
