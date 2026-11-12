using HHG.Common.Runtime;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    // Add, duplicate, rename, delete, reorder and repair sub-assets held in SubAssetList fields.
    // Changes go through SerializedObject so the owner's OnValidate runs afterwards.
    public static class SubAssetUtil
    {
        private const string itemsName = "items";
        private const BindingFlags fieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public static List<(FieldInfo field, System.Type type)> GetLists(Object owner)
        {
            List<(FieldInfo, System.Type)> lists = new List<(FieldInfo, System.Type)>();

            for (System.Type type = owner != null ? owner.GetType() : null; type != null && type != typeof(ScriptableObject); type = type.BaseType)
            {
                foreach (FieldInfo field in type.GetFields(fieldFlags))
                {
                    System.Type fieldType = field.FieldType;

                    if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(SubAssetList<>))
                    {
                        lists.Add((field, fieldType.GetGenericArguments()[0]));
                    }
                }
            }

            return lists;
        }

        public static bool IsInList(Object subAsset)
        {
            return TryFind(subAsset, out _, out _, out _);
        }

        public static ScriptableObject Add(Object owner, string fieldName, System.Type type)
        {
            SerializedObject serializedObject = new SerializedObject(owner);
            SerializedProperty items = serializedObject.FindProperty(fieldName).FindPropertyRelative(itemsName);

            ScriptableObject child = ScriptableObject.CreateInstance(type);
            child.name = GetUniqueName(owner, $"New {ObjectNames.NicifyVariableName(type.Name)}");
            AssetDatabase.AddObjectToAsset(child, owner);

            Insert(items, items.arraySize, child);
            Save(owner);
            Selection.activeObject = child;
            return child;
        }

        public static void Duplicate(ScriptableObject subAsset)
        {
            if (!TryFind(subAsset, out Object owner, out SerializedProperty items, out int index)) return;

            ScriptableObject copy = Object.Instantiate(subAsset);
            copy.name = GetUniqueName(owner, $"{subAsset.name} Copy");
            AssetDatabase.AddObjectToAsset(copy, owner);

            Insert(items, index + 1, copy);
            Save(owner);
            Selection.activeObject = copy;
        }

        public static void Rename(ScriptableObject subAsset, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName)) return;

            Object owner = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(subAsset));
            subAsset.name = newName.Trim();
            EditorUtility.SetDirty(subAsset);
            Save(owner);
        }

        public static void Delete(ScriptableObject subAsset)
        {
            Object owner = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(subAsset));

            if (TryFind(subAsset, out _, out SerializedProperty items, out int index))
            {
                // Clear first: deleting a non-null object reference element only nulls it
                items.GetArrayElementAtIndex(index).objectReferenceValue = null;
                items.DeleteArrayElementAtIndex(index);
                items.serializedObject.ApplyModifiedProperties();
            }

            Object.DestroyImmediate(subAsset, true);
            Save(owner);
        }

        public static void Move(ScriptableObject subAsset, int offset)
        {
            if (!TryFind(subAsset, out Object owner, out SerializedProperty items, out int index)) return;

            int newIndex = index + offset;

            if (newIndex < 0 || newIndex >= items.arraySize) return;

            items.MoveArrayElement(index, newIndex);
            items.serializedObject.ApplyModifiedProperties();
            Save(owner);
        }

        // Drops empty entries and lists any matching sub-assets that are missing. Never renames or deletes.
        public static void Repair(Object owner)
        {
            SerializedObject serializedObject = new SerializedObject(owner);
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(owner));
            HashSet<Object> listed = new HashSet<Object>();

            foreach ((FieldInfo field, System.Type type) in GetLists(owner))
            {
                SerializedProperty items = serializedObject.FindProperty(field.Name).FindPropertyRelative(itemsName);

                for (int i = items.arraySize - 1; i >= 0; i--)
                {
                    Object item = items.GetArrayElementAtIndex(i).objectReferenceValue;

                    if (item == null) items.DeleteArrayElementAtIndex(i);
                    else listed.Add(item);
                }

                foreach (Object subAsset in subAssets)
                {
                    if (type.IsInstanceOfType(subAsset) && listed.Add(subAsset))
                    {
                        items.InsertArrayElementAtIndex(items.arraySize);
                        items.GetArrayElementAtIndex(items.arraySize - 1).objectReferenceValue = subAsset;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
            Save(owner);
        }

        private static bool TryFind(Object subAsset, out Object owner, out SerializedProperty items, out int index)
        {
            owner = subAsset != null && AssetDatabase.IsSubAsset(subAsset) ? AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(subAsset)) : null;
            items = null;
            index = -1;

            if (owner == null) return false;

            SerializedObject serializedObject = new SerializedObject(owner);

            foreach ((FieldInfo field, _) in GetLists(owner))
            {
                items = serializedObject.FindProperty(field.Name).FindPropertyRelative(itemsName);

                for (int i = 0; i < items.arraySize; i++)
                {
                    if (items.GetArrayElementAtIndex(i).objectReferenceValue == subAsset)
                    {
                        index = i;
                        return true;
                    }
                }
            }

            items = null;
            return false;
        }

        private static void Insert(SerializedProperty items, int index, Object value)
        {
            items.InsertArrayElementAtIndex(index);
            items.GetArrayElementAtIndex(index).objectReferenceValue = value;
            items.serializedObject.ApplyModifiedProperties();
        }

        private static string GetUniqueName(Object owner, string baseName)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (Object subAsset in AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(owner)))
            {
                if (subAsset != null) names.Add(subAsset.name);
            }

            string name = baseName;

            for (int i = 2; names.Contains(name); i++)
            {
                name = $"{baseName} {i}";
            }

            return name;
        }

        private static void Save(Object owner)
        {
            EditorUtility.SetDirty(owner);
            AssetDatabase.SaveAssets();

            // Reimport so the Project window shows new names and order
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(owner));
        }
    }
}
