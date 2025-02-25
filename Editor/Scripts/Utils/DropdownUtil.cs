using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace HHG.Common.Runtime
{
    public static class DropdownUtil
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
            IEnumerable<Type> types = typeof(UnityEngine.Object).FindSubclasses().Where(typeFilter);

            foreach (var type in types)
            {
#if UNITY_EDITOR
                string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");
                string[] paths = guids.Select(g => AssetDatabase.GUIDToAssetPath(g)).ToArray();
                T[] objs = paths.SelectMany(p => GetAssetsAtPath(p, objectFilter)).ToArray();
#else
                var objs = Resources.LoadAll(string.Empty, type).Cast<T>().Where(objectFilter);
#endif
                objectsRetval.AddRange(objs);
            }

            objects = objectsRetval;
            options = objects.Select(e => e == null ? "None" : FormatChoiceText(e.name));
        }

        private static IEnumerable<T> GetAssetsAtPath<T>(string p, Func<T, bool> objectFilter) where T : UnityEngine.Object
        {
            // Calling AssetDatabase.LoadAllAssetsAtPath on a SceneAsset path does not work - it throws an error
            // We need to check if the type is SceneAsset and, if so, use AssetDatabase.LoadMainAssetAtPath instead
            if (typeof(SceneAsset).IsAssignableFrom(typeof(T)))
            {
                return Enumerable.Empty<T>().Append(AssetDatabase.LoadMainAssetAtPath(p)).OfType<T>().Where(objectFilter);
            }
            else
            {
                return AssetDatabase.LoadAllAssetsAtPath(p).OfType<T>().Where(objectFilter);
            }
        }

        public static string FormatChoiceText(string choice)
        {
            // Make sure to replace " - " but not "-" since we don't 
            // want to split for hyphenated words or negative numbers
            // Also replace with " / " with a space on each side since
            // extra spaces get removed anyways and it'll look nicer
            return Regex.Replace(choice.
                       ReplaceMany(new string[] { " - ", ",", "[", "(", "{" }, " / ").
                       ReplaceMany(new string[] { "]", ")", "}" }, string.Empty), @"\s+", " ");
        }
    }
}