using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HHG.Common.Runtime
{
    public abstract class CheatManagerBase : MonoBehaviour
    {
        private const Key toggleCheatsKey = Key.F12;

        private static readonly Key[] modifierKeys = new Key[]
        {
            Key.LeftShift,
            Key.RightShift,
            Key.LeftCtrl,
            Key.RightCtrl,
            Key.LeftAlt,
            Key.RightAlt
        };

        [SerializeField] private bool logsEnabled;

        private Dictionary<Key, Cheat> cheats = new Dictionary<Key, Cheat>();
        private bool cheatsEnabled = false;

        private struct Cheat
        {
            public Action Action;
            public string Description;
            public string MethodName;

            public Cheat(Action action, string description, string methodName)
            {
                Action = action;
                Description = description;
                MethodName = methodName;
            }
        }

        protected virtual void Awake()
        {
            // Enabled cheats by default for the editor and debug builds
            cheatsEnabled = Application.isEditor || Debug.isDebugBuild;

            RegisterCheats();
        }

        private void RegisterCheats()
        {
            MethodInfo[] methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (MethodInfo method in methods)
            {
                CheatAttribute attribute = method.GetCustomAttribute<CheatAttribute>();
                if (attribute != null)
                {
                    cheats[attribute.Key] = new Cheat
                    (
                        (Action)Delegate.CreateDelegate(typeof(Action), this, method),
                        attribute.Description,
                        method.Name.ToNicified()
                    );
                }
            }
        }

        private void Update()
        {
            if (Keyboard.current[toggleCheatsKey].wasPressedThisFrame)
            {
                cheatsEnabled = !cheatsEnabled;

                Log($"Cheats enabled: {cheatsEnabled}");
            }

            if (!cheatsEnabled)
            {
                return;
            }

            bool modifier = modifierKeys.Any(key => Keyboard.current[key].isPressed);

            if (!modifier)
            {
                return;
            }

            foreach (KeyValuePair<Key, Cheat> kvpair in cheats)
            {
                Key key = kvpair.Key;
                Cheat cheat = kvpair.Value;

                if (Keyboard.current[key].wasPressedThisFrame)
                {
                    cheat.Action.Invoke();

                    Log($"Cheat activated: {cheat.MethodName.ToNicified()} ({key})");
                }
            }
        }

        [ContextMenu("Log Help Text")]
        private void LogHelpText()
        {
            Debug.Log(GetHelpText());
        }

        public string GetHelpText()
        {
            return "Cheats:\n" + string.Join("\n", cheats.
                    OrderBy(c => c.Key).
                    Select(c => $"  • Shift + {c.Key} — {c.Value.MethodName}{(string.IsNullOrWhiteSpace(c.Value.Description) ? "" : $": {c.Value.Description}")}"));
        }

        private void Log(object message)
        {
            if (logsEnabled)
            {
                Debug.Log(message);
            }
        }
    }
}
