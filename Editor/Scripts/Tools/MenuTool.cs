using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HHG.Common.Editor
{
    public static class MenuTool
    {
        private static readonly Type type = Type.GetType("UnityEditor.Menu, UnityEditor");

        public static void SetChecked(string menuPath, bool isChecked)
        {
            CallInternalMethod(nameof(SetChecked), menuPath, isChecked);
        }

        public static bool GetChecked(string menuPath)
        {
            return (bool)CallInternalMethod(nameof(GetChecked), menuPath);
        }

        public static bool GetEnabled(string menuPath)
        {
            return (bool)CallInternalMethod(nameof(GetEnabled), menuPath);
        }

        public static void GetMenuItemDefaultShortcuts(List<string> outItemNames, List<string> outItemDefaultShortcuts)
        {
            CallInternalMethod(nameof(GetMenuItemDefaultShortcuts), outItemNames, outItemDefaultShortcuts);
        }

        public static void SetHotkey(string menuPath, string shortcut)
        {
            CallInternalMethod(nameof(SetHotkey), menuPath, shortcut);
        }

        public static string[] ExtractSubmenus(string menuPath)
        {
            return (string[])CallInternalMethod(nameof(ExtractSubmenus), menuPath);
        }

        public static void AddMenuItem(string name, string shortcut, bool isChecked, int priority, Action execute, Func<bool> validate)
        {
            CallInternalMethod(nameof(AddMenuItem), name, shortcut, isChecked, priority, execute, validate);
        }

        public static void RemoveMenuItem(string menuPath)
        {
            CallInternalMethod(nameof(RemoveMenuItem), menuPath);
        }

        public static void AddSeparator(string name, int priority)
        {
            CallInternalMethod(nameof(AddSeparator), name, priority);
        }

        public static void RebuildAllMenus()
        {
            CallInternalMethod(nameof(RebuildAllMenus));
        }

        public static int FindHotkeyStartIndex(string menuPath)
        {
            return (int)CallInternalMethod(nameof(FindHotkeyStartIndex), menuPath);
        }

        public static bool MenuItemExists(string menuPath)
        {
            return (bool)CallInternalMethod(nameof(MenuItemExists), menuPath);
        }

        private static object CallInternalMethod(string methodName, params object[] parameters)
        {
            MethodInfo method = type?.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            if (method != null)
            {
                return method.Invoke(null, parameters);
            }
            else
            {
                Debug.LogError($"Method {methodName} not found in UnityEditor.Menu");
                return null;
            }
        }
    }
}
