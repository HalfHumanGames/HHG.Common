using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class SetParamStateMachineBehavior : StateMachineBehaviour
    {
        [SerializeField] private SetBool[] bools;
        [SerializeField] private SetFloat[] floats;
        [SerializeField] private SetInt[] ints;
        [SerializeField] private string[] triggers;

        private bool hashesSet;
        private int[] triggerHashes;
        private int frameCount;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            TrySetHashes();
            SetVariables(animator, WhenFlags.Enter);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Do in OnStateUpdate since doing on OnStateExit will
            // not work since a transition is already in progress
            TrySetHashes();
            ResetTriggers(animator);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (frameCount == Time.frameCount)
            {
                return;
            }
            frameCount = Time.frameCount;
            TrySetHashes();
            SetVariables(animator, WhenFlags.Exit);
            // Force update the animator so the parameters get re-evaluated
            animator.Update(Time.deltaTime);
        }

        private void SetVariables(Animator animator, WhenFlags now)
        {
            if (bools != null && bools.Length > 0)
            {
                foreach (SetBool b in bools)
                {
                    if (b.When.HasFlag(now))
                    {
                        animator.SetBool(b.Hash, b.Value);
                    }
                }
            }
            if (floats != null && floats.Length > 0)
            {
                foreach (SetFloat f in floats)
                {
                    if (f.When.HasFlag(now))
                    {
                        animator.SetFloat(f.Hash, f.Value);
                    }
                }
            }
            if (ints != null && ints.Length > 0)
            {
                foreach (SetInt i in ints)
                {
                    if (i.When.HasFlag(now))
                    {
                        animator.SetInteger(i.Hash, i.Value);
                    }
                }
            }
            ResetTriggers(animator);
        }

        private void ResetTriggers(Animator animator)
        {
            if (triggers != null && triggers.Length > 0)
            {
                foreach (int t in triggerHashes)
                {
                    animator.ResetTrigger(t);
                }
            }
        }

        private void TrySetHashes()
        {
            if (hashesSet)
            {
                return;
            }
            hashesSet = true;
            if (bools != null && bools.Length > 0)
            {
                foreach (SetBool b in bools)
                {
                    b.Hash = Animator.StringToHash(b.Name);
                }
            }
            if (floats != null && floats.Length > 0)
            {
                foreach (SetFloat f in floats)
                {
                    f.Hash = Animator.StringToHash(f.Name);
                }
            }
            if (ints != null && ints.Length > 0)
            {
                foreach (SetInt i in ints)
                {
                    i.Hash = Animator.StringToHash(i.Name);
                }
            }
            if (triggers != null && triggers.Length > 0)
            {
                triggerHashes = new int[triggers.Length];
                for (int i = 0; i < triggers.Length; i++)
                {
                    triggerHashes[i] = Animator.StringToHash(triggers[i]);
                }
            }
        }

        public enum WhenFlags
        {
            Exit, // Made this default since that's the original use case
            Enter,
            //Update = 4 // I cannot see a good use case for this
        }

        public class SetParamBase
        {
            public string Name;
            [NonSerialized]
            public int Hash;
            public WhenFlags When;
        }

        [Serializable]
        public class SetBool : SetParamBase
        {
            public bool Value;
        }

        [Serializable]
        public class SetFloat : SetParamBase
        {
            public float Value;
        }

        [Serializable]
        public class SetInt : SetParamBase
        {
            public int Value;
        }
    }

}