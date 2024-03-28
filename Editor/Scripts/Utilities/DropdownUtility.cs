using HHG.Common.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHG.SpawnSystem
{
    public static class DropdownUtility
    {
        public static void GetChoiceList<T>(
            ref List<T> objects,
            System.Func<T, bool> objectFilter = null,
            System.Func<System.Type, bool> typeFilter = null
        ) where T : Object
        {
            GetChoicesEnumerable(out IEnumerable<T> objs, out IEnumerable<string> str, objectFilter, typeFilter);
            objects.Clear();
            objects.AddRange(objs);
        }

        public static void GetChoiceList<T>(
            ref List<T> objects,
            ref List<string> options,
            System.Func<T, bool> objectFilter = null,
            System.Func<System.Type, bool> typeFilter = null
        ) where T : Object
        {
            GetChoicesEnumerable(out IEnumerable<T> objs, out IEnumerable<string> str, objectFilter, typeFilter);
            objects.Clear();
            options.Clear();
            objects.AddRange(objs);
            options.AddRange(str);
        }

        public static void GetChoiceArray<T>(
            ref T[] objects,
            System.Func<T, bool> objectFilter = null,
            System.Func<System.Type, bool> typeFilter = null
        ) where T : Object
        {
            GetChoicesEnumerable(out IEnumerable<T> objs, out IEnumerable<string> str, objectFilter, typeFilter);
            System.Array.Resize(ref objects, objs.Count());

            int i = 0;
            foreach (T obj in objs)
            {
                objects[i++] = obj;
            }
        }

        public static void GetChoiceArray<T>(
            ref T[] objects,
            ref string[] options,
            System.Func<T, bool> objectFilter = null,
            System.Func<System.Type, bool> typeFilter = null
        ) where T : Object
        {
            GetChoicesEnumerable(out IEnumerable<T> objs, out IEnumerable<string> opts, objectFilter, typeFilter);
            System.Array.Resize(ref objects, objs.Count());
            System.Array.Resize(ref options, opts.Count());

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

        private static void GetChoicesEnumerable<T>(
            out IEnumerable<T> objects,
            out IEnumerable<string> options,
            System.Func<T, bool> objectFilter = null,
            System.Func<System.Type, bool> typeFilter = null
        ) where T : Object
        {
            objectFilter ??= _ => true;
            typeFilter ??= _ => true;
            System.Type type = typeof(ScriptableObject).FindSubclasses().FirstOrDefault(typeFilter);
            objects = Resources.LoadAll(string.Empty, type).Select(r => r as T).Where(objectFilter).Prepend(null);
            options = objects.Select(e => e == null ? "None" : e.name);
        }
    }
}