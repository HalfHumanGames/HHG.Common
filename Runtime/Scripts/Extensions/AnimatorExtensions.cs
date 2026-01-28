using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class AnimatorExtensions
    {
        public static void Offset(this Animator animator, int layer = 0)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layer);
            float offset = Random.Range(0f, 1f);
            animator.Play(info.fullPathHash, layer, offset);
        }
    }
}