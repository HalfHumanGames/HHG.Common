using HHG.Common.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Sample
{
    public class SampleCheatManager : CheatManagerBase
    {
        [Cheat(Key.A)]
        public void SampleCheatA()
        {
            Debug.Log("Sample Cheat A");
        }

        [Cheat(Key.B, "This is a sample cheat description")]
        public void SampleCheatB()
        {
            Debug.Log("Sample Cheat B");
        }
    }
}