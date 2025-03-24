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

        [SerializeField] private bool logsEnabled;

        private Dictionary<Key, (Action Action, string Description, string MethodName)> cheats = new();
        private bool cheatsEnabled = false;

        protected virtual void Awake()
        {
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
                    cheats[attribute.Key] =
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

            foreach (var cheat in cheats)
            {
                if (Keyboard.current[cheat.Key].wasPressedThisFrame)
                {
                    cheat.Value.Action.Invoke();

                    Log($"Cheat activated: {cheat.Value.MethodName.ToNicified()} ({cheat.Key})");
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
