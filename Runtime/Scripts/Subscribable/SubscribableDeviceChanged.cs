using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class SubscribableDeviceChanged : ISubscribable
    {
        private System.Action callback;

        public void Subscribe(System.Action callback)
        {
            this.callback = callback;
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        public void Unsubscribe(System.Action callback)
        {
            this.callback = callback;
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        private void OnDeviceChange(InputDevice arg1, InputDeviceChange arg2)
        {
            callback?.Invoke();
        }
    }
}