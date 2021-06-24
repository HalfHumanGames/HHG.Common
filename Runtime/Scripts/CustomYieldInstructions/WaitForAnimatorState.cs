using UnityEngine;

namespace HHG.Common
{
    public class WaitForAnimatorState : CustomYieldInstruction
    {
        public override bool keepWaiting {
            get {
                AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName(State) || stateInfo.IsTag(State))
                {
                    return stateInfo.normalizedTime < NormalizedTime;
                }
                return true;
            }
        }

        public Animator Animator { get; set; }
        public string State { get; set; }
        public float NormalizedTime { get; set; }

        public WaitForAnimatorState(Animator animator, string state, float normalizedTime = 0)
        {
            Animator = animator;
            State = state;
            NormalizedTime = normalizedTime;
        }
    }
}