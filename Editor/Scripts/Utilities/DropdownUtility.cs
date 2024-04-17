using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace HHG.Common.Runtime
{
    public static class DropdownUtility
    {
        public static void GetChoiceList<T>(ref List<T> objects, Func<Type, bool> typeFilter = null, Func<T, bool> objectFilter = null) where T : UnityEngine.Object
        {
            GetChoicesEnumerable(out IEnumerable<T> objs, out IEnumerable<string> str, typeFilter, objectFilter);
            objects.Clear();
            objects.AddRange(objs);
        }

        public static void GetChoiceList<T>(ref List<T> objects, ref List<string> options, Func<Type, bool> typeFilter = null, Func<T, bool> objectFilter = null) where T : UnityEngine.Object
        {
            GetChoicesEnumerable(out IEnumerable<T> objs, out IEnumerable<string> str, typeFilter, objectFilter);
            objects.Clear();
            options.Clear();
            objects.AddRange(objs);
            options.AddRange(str);
        }

        public static void GetChoiceArray<T>(ref T[] objects, Func<Type, bool> typeFilter = null, Func<T, bool> objectFilter = null) where T : UnityEngine.Object
        {
            GetChoicesEnumerable(out IEnumerable<T> objs, out IEnumerable<string> str, typeFilter, objectFilter);
            Array.Resize(ref objects, objs.Count());

            int i = 0;
            foreach (T obj in objs)
            {
                objects[i++] = obj;
            }
        }

        public static void GetChoiceArray<T>(ref T[] objects, ref string[] options, Func<Type, bool> typeFilter = null, Func<T, bool> objectFilter = null) where T : UnityEngine.Object
        {
            GetChoicesEnumerable(out IEnumerable<T> objs, out IEnumerable<string> opts, typeFilter, objectFilter);
            Array.Resize(ref objects, objs.Count());
            Array.Resize(ref options, opts.Count());

            int i = 0;
            foreach (T obj in objs)
            {
                objects[i++] = obj;
            }

            i = 0;
            foreach (string opt in opts)
            {
                options[i++] = opt;
            }
        }

        private static void GetChoicesEnumerable<T>(out IEnumerable<T> objects, out IEnumerable<string> options, Func<Type, bool> typeFilter = null, Func<T, bool> objectFilter = null) where T : UnityEngine.Object
        {
            objectFilter ??= _ => true;
            typeFilter ??= _ => true;
            List<T> objectsRetval = new List<T>() { null };
            List<string> optionsRetval = new List<string>();
            var types = typeof(ScriptableObject).FindSubclasses().Where(typeFilter);
            foreach (var type in types)
            {
                var objs = Resources.LoadAll(string.Empty, type).Cast<T>().Where(objectFilter);
                objectsRetval.AddRange(objs);
            }
            objects = objectsRetval;
            options = objects.Select(e => e == null ? "None" : FormatChoiceText(e.name));
        }

        public static string FormatChoiceText(string choice)
        {
            return choice.
                   ReplaceMany(new char[] { '.', '-', '[', '(', '{' }, '/').
                   ReplaceMany(new char[] { ' ', ']', ')', '}' }, string.Empty);
        }
    }
}