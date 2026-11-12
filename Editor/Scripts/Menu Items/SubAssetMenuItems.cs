using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    // Project window right-click menu for SubAssetList sub-assets (Assets > Tools > Sub-Assets)
    public static class SubAssetMenuItems
    {
        private const string root = "Assets/Tools/Sub-Assets/";

        private static Object selected => Selection.activeObject;
        private static ScriptableObject selectedSubAsset => selected is ScriptableObject subAsset && SubAssetUtil.IsInList(subAsset) ? subAsset : null;

        [MenuItem(root + "Add", false, 0)]
        private static void Add()
        {
            Object owner = selected;
            List<(FieldInfo field, System.Type type)> lists = SubAssetUtil.GetLists(owner);

            if (lists.Count == 1)
            {
                SubAssetUtil.Add(owner, lists[0].field.Name, lists[0].type);
                return;
            }

            GenericMenu menu = new GenericMenu();

            foreach ((FieldInfo field, System.Type type) in lists)
            {
                menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(field.Name)), false, () => SubAssetUtil.Add(owner, field.Name, type));
            }

            menu.ShowAsContext();
        }

        [MenuItem(root + "Add", true)]
        private static bool ValidateAdd() => IsOwner(selected);

        [MenuItem(root + "Rename", false, 20)]
        private static void Rename() => SubAssetRenamePopup.Open(selectedSubAsset);

        [MenuItem(root + "Rename", true)]
        private static bool ValidateRename() => selectedSubAsset != null;

        [MenuItem(root + "Duplicate", false, 21)]
        private static void Duplicate() => SubAssetUtil.Duplicate(selectedSubAsset);

        [MenuItem(root + "Duplicate", true)]
        private static bool ValidateDuplicate() => selectedSubAsset != null;

        [MenuItem(root + "Delete", false, 22)]
        private static void Delete()
        {
            ScriptableObject subAsset = selectedSubAsset;

            if (EditorUtility.DisplayDialog("Delete Sub-Asset", $"Delete '{subAsset.name}'?\n\nAnything referencing it will lose the reference. This can't be undone.", "Delete", "Cancel"))
            {
                SubAssetUtil.Delete(subAsset);
            }
        }

        [MenuItem(root + "Delete", true)]
        private static bool ValidateDelete() => selectedSubAsset != null;

        [MenuItem(root + "Move Up", false, 40)]
        private static void MoveUp() => SubAssetUtil.Move(selectedSubAsset, -1);

        [MenuItem(root + "Move Up", true)]
        private static bool ValidateMoveUp() => selectedSubAsset != null;

        [MenuItem(root + "Move Down", false, 41)]
        private static void MoveDown() => SubAssetUtil.Move(selectedSubAsset, 1);

        [MenuItem(root + "Move Down", true)]
        private static bool ValidateMoveDown() => selectedSubAsset != null;

        [MenuItem(root + "Repair List", false, 60)]
        private static void Repair() => SubAssetUtil.Repair(selected);

        [MenuItem(root + "Repair List", true)]
        private static bool ValidateRepair() => IsOwner(selected);

        private static bool IsOwner(Object obj)
        {
            return obj != null && AssetDatabase.IsMainAsset(obj) && SubAssetUtil.GetLists(obj).Count > 0;
        }
    }
}
