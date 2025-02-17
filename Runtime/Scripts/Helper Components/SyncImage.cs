using UnityEngine;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public class SyncImage : MonoBehaviour
    {
        [SerializeField] private Image target;

        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void LateUpdate()
        {
            image.sprite = target.sprite;
        }
    }
}