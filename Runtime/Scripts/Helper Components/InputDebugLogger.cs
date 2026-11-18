using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;

namespace HHG.Common.Runtime
{
    // Logs device, pairing, player and focus changes for debugging input issues.
    // Every line is prefixed with [InputDebug] in the console, Editor.log and Player.log.
    public class InputDebugLogger : MonoBehaviour
    {
        [SerializeField] private bool logPresses = true;
        [SerializeField, Min(0f)] private float silenceThreshold = 3f;

        private readonly Dictionary<int, float> lastEventTimes = new Dictionary<int, float>();
        private readonly Dictionary<int, (bool active, bool mapEnabled, string scheme)> playerStates = new Dictionary<int, (bool, bool, string)>();
        private Action<InputEventPtr, InputDevice> eventListener;

        private void OnEnable()
        {
            eventListener ??= OnEvent;
            InputSystem.onEvent += eventListener;
            InputSystem.onDeviceChange += OnDeviceChange;
            InputUser.onChange += OnUserChange;
            Application.focusChanged += OnFocusChanged;

            Log($"Enabled | InputSystem {InputSystem.version} | backgroundBehavior={InputSystem.settings.backgroundBehavior} | runInBackground={Application.runInBackground} | editor={Application.isEditor}");
            LogState("Enabled");
        }

        private void Update()
        {
            LogPlayerChanges();

            if (logPresses) LogGamepadPresses();
        }

        private void LogPlayerChanges()
        {
            foreach (PlayerInput player in PlayerInput.all)
            {
                bool active = player.inputIsActive;
                bool mapEnabled = player.currentActionMap != null && player.currentActionMap.enabled;
                string scheme = player.currentControlScheme;

                if (!playerStates.TryGetValue(player.playerIndex, out var state) || state.active != active || state.mapEnabled != mapEnabled || state.scheme != scheme)
                {
                    playerStates[player.playerIndex] = (active, mapEnabled, scheme);
                    Log($"Player changed | {DescribePlayer(player)}");
                }
            }
        }

        private void LogGamepadPresses()
        {
            foreach (Gamepad gamepad in Gamepad.all)
            {
                foreach (InputControl control in gamepad.allControls)
                {
                    if (control is ButtonControl button && button.wasPressedThisFrame)
                    {
                        Log($"Pressed {gamepad.name}/{button.name} (enabled={gamepad.enabled}) | owner {DescribeOwner(gamepad)} | UI {DescribeUI()}");
                    }
                }
            }
        }

        private void OnEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (device is not Gamepad) return;
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return;

            float now = Time.realtimeSinceStartup;
            bool seen = lastEventTimes.TryGetValue(device.deviceId, out float last);

            if (!seen || now - last > silenceThreshold)
            {
                string gap = seen ? $"{now - last:0.0}s of silence" : "first event";
                Log($"Events from {device.name} (id {device.deviceId}, enabled={device.enabled}) after {gap}");
            }

            lastEventTimes[device.deviceId] = now;
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            Log($"Device {change}: {device.name} (id {device.deviceId}, enabled={device.enabled}, added={device.added}) | owner {DescribeOwner(device)}");

            if (change == InputDeviceChange.Removed || change == InputDeviceChange.Added || change == InputDeviceChange.Disabled)
            {
                LogState($"Device {change}");
            }
        }

        private void OnUserChange(InputUser user, InputUserChange change, InputDevice device)
        {
            PlayerInput player = PlayerInput.all.FirstOrDefault(p => p.user == user);
            string playerName = player != null ? $"P{player.playerIndex}" : "no player";
            string deviceName = device != null ? device.name : "-";
            Log($"User {change}: user {user.index} ({playerName}) device={deviceName} scheme={user.controlScheme?.name}");
        }

        private void OnFocusChanged(bool focused)
        {
            LogState(focused ? "Focus gained" : "Focus lost");
        }

        private void LogState(string reason)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"State ({reason}) | focused={Application.isFocused} | Gamepad.current={(Gamepad.current != null ? Gamepad.current.name : "none")}");

            foreach (InputDevice device in InputSystem.devices)
            {
                sb.Append($"\n    {device.name} id={device.deviceId} enabled={device.enabled} | owner {DescribeOwner(device)}");
            }

            foreach (InputDevice device in InputSystem.disconnectedDevices)
            {
                sb.Append($"\n    (disconnected) {device.name} id={device.deviceId}");
            }

            foreach (PlayerInput player in PlayerInput.all)
            {
                sb.Append($"\n    {DescribePlayer(player)}");
            }

            sb.Append($"\n    UI {DescribeUI()}");
            Log(sb.ToString());
        }

        private string DescribeOwner(InputDevice device)
        {
            PlayerInput player = PlayerInput.FindFirstPairedToDevice(device);
            return player != null ? $"P{player.playerIndex}" : "unpaired";
        }

        private string DescribePlayer(PlayerInput player)
        {
            InputActionMap map = player.currentActionMap;
            string mapName = map != null ? $"{map.name}({(map.enabled ? "on" : "off")})" : "none";
            string devices = string.Join(", ", player.devices.Select(d => d.name));
            string lost = player.user.valid ? string.Join(", ", player.user.lostDevices.Select(d => d.name)) : "invalid user";
            return $"P{player.playerIndex} scheme={player.currentControlScheme} inputActive={player.inputIsActive} map={mapName} devices=[{devices}] lost=[{lost}]";
        }

        private string DescribeUI()
        {
            EventSystem eventSystem = EventSystem.current;

            if (eventSystem == null) return "no EventSystem";

            InputSystemUIInputModule module = eventSystem.GetComponent<InputSystemUIInputModule>();

            if (module == null) return "no InputSystemUIInputModule";

            PlayerInput owner = PlayerInput.all.FirstOrDefault(p => p.actions == module.actionsAsset);
            string ownerName = owner != null ? $"P{owner.playerIndex}" : "shared asset";
            GameObject selected = eventSystem.currentSelectedGameObject;
            return $"actions={ownerName} move={DescribeAction(module.move)} submit={DescribeAction(module.submit)} cancel={DescribeAction(module.cancel)} selected={(selected != null ? selected.name : "none")}";
        }

        private string DescribeAction(InputActionReference reference)
        {
            InputAction action = reference != null ? reference.action : null;
            return action != null ? $"{action.name}({(action.enabled ? "on" : "off")})" : "none";
        }

        private static void Log(string message)
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "[InputDebug] {0:0.00} {1}", Time.realtimeSinceStartup, message);
        }

        private void OnDisable()
        {
            InputSystem.onEvent -= eventListener;
            InputSystem.onDeviceChange -= OnDeviceChange;
            InputUser.onChange -= OnUserChange;
            Application.focusChanged -= OnFocusChanged;
        }
    }
}
