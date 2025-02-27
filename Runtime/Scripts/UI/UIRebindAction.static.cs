using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public partial class UIRebindAction : MonoBehaviour
    {
        private static List<UIRebindAction> rebindActions;
       
        // When the action system re-resolves bindings, we want to update our UI in response. While this will
        // also trigger from changes we made ourselves, it ensures that we react to changes made elsewhere. If
        // the user changes keyboard layout, for example, we will get a BoundControlsChanged notification and
        // will update our UI to reflect the current keyboard layout.
        private static void OnActionChange(object actionWeak, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged)
            {
                return;
            }

            InputAction action = actionWeak as InputAction;
            InputActionMap actionMap = action?.actionMap ?? actionWeak as InputActionMap;
            InputActionAsset actionAsset = actionMap?.asset ?? actionWeak as InputActionAsset;

            for (int i = 0; i < rebindActions.Count; ++i)
            {
                UIRebindAction component = rebindActions[i];
                InputAction referencedAction = component.ActionReference?.action;

                if (referencedAction == null)
                {
                    continue;
                }

                if (referencedAction == action ||
                    referencedAction.actionMap == actionMap ||
                    referencedAction.actionMap?.asset == actionAsset)
                {
                    component.UpdateBindingDisplay();
                }
            }
        }
    }
}
