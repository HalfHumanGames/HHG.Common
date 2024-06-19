using HHG.Common.Runtime;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    public class RenameTitleCaseTool : EditorWindow
    {
        [MenuItem("Assets/Tools/Rename Title Case")]
        public static void RenameSubstring_Asset()
        {
            foreach (Object obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                string newName = obj.name.ToTitleCase();
                AssetDatabase.RenameAsset(path, newName);
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("GameObject/Tools/Rename Title Case")]
        public static void RenameSubstring_GameObject()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.name = obj.name.ToTitleCase();
            }
        }

        [MenuItem("Assets/Tools/Rename Title Case", true)]
        public static bool CanRenameTitleCase()
        {
            if (Selection.activeObject == null)
            {
                return false;
            }

            if (Selection.objects.Length <= 1)
            {
                return false;
            }

            return true;
        }
    }
}