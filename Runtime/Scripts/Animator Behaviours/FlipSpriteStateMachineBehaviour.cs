using UnityEngine;

namespace HHG.Common.Runtime
{
    public class FlipSpriteStateMachineBehaviour : StateMachineBehaviour
    {
        [SerializeField] private bool flipX;
        [SerializeField] private bool flipY;

        private Lazy<SpriteRenderer> _spriteRenderer = new Lazy<SpriteRenderer>();
        private SpriteRenderer spriteRenderer;
        private bool originalflipX;
        private bool originalflipY;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            originalflipX = flipX; 
            originalflipY = flipY;    
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            spriteRenderer ??= _spriteRenderer.FromComponent(animator);
            spriteRenderer.flipX = flipX;
            spriteRenderer.flipY = flipY;
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            spriteRenderer.flipX = originalflipX;
            spriteRenderer.flipY = originalflipY;
        }
    }
}