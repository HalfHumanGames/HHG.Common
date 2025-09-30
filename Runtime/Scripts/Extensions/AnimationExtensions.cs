using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class AnimationExtensions
    {
        public static void Pause(this Animation anim)
        {
            foreach (AnimationState state in anim)
            {
                state.speed = 0f;
            }
        }

        public static void Resume(this Animation anim)
        {
            foreach (AnimationState state in anim)
            {
                state.speed = 1f;
            }
        }
    }
}