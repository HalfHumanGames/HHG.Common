using UnityEngine;

namespace HHG.Common.Runtime
{
    public class SyncSpriteRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer target;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            spriteRenderer.sprite = target.sprite;
        }
    }
}