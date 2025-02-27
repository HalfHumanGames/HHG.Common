using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class InputActionBinding
    {
        [FormerlySerializedAs("action")] public InputActionReference Action;
        [FormerlySerializedAs("bindingId")] public string BindingId;
    }
}