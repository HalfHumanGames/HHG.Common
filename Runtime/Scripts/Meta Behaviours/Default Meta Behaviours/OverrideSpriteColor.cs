using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class OverrideSpriteColor : MetaBehaviour
    {
        [SerializeField] private Color color;

        private SpriteRenderer spriteRenderer;

        public override void Start()
        {
            if (gameObject.TryGetComponentInChildren(out spriteRenderer))
            {
                spriteRenderer.color = color;
            }
        }

        public override void Update()
        {
            if (spriteRenderer)
            {
                spriteRenderer.color = color;
            }
        }

        public override void OnDestroy()
        {
            if (spriteRenderer)
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}