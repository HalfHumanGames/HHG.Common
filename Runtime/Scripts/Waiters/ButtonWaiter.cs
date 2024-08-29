using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public static class ButtonWaiter
    {
        public static IEnumerator WaitForButtonPress(InputAction action)
        {
            while (!action.IsPressed())
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}