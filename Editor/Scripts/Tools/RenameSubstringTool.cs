using UnityEngine;
using UnityEditor;

namespace HHG.Common.Editor
{
    public class RenameSubstringTool : EditorWindow
    {
        private RenameMode mode;
        private string findString;
        private string replaceString;

        public enum RenameMode
        {
            Asset,
            GameObject
        }

        [MenuItem("Assets/Tools/Rename Substring")]
        public static void RenameSubstring_Asset()
        {
            GetWindow<RenameSubstringTool>(true, "Rename Assets", true).mode = RenameMode.Asset;
        }

        [MenuItem("GameObject/Tools/Rename Substring")]
        public static void RenameSubstring_GameObject()
        {
            GetWindow<RenameSubstringTool>(true, "Rename Assets", true).mode = RenameMode.GameObject;
        }

        [MenuItem("Assets/Tools/Rename Substring", true)]
        public static bool CanRenameSubstring()
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

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Rename assets by replacing a substring.", MessageType.None);
            findString = EditorGUILayout.TextField("Find", findString);
            replaceString = EditorGUILayout.TextField("Replace", replaceString);
            if (GUILayout.Button("Rename"))
            {
                switch (mode)
                {
                    case RenameMode.Asset:
                        RenameSelectedAssets(findString, replaceString);
                        break;
                    case RenameMode.GameObject:
                        RenameSelectedGameObjects(findString, replaceString);
                        break;
                }
                Close();
            }
        }

        private static void RenameSelectedAssets(string findString, string replaceString)
        {
            foreach (Object obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                string newName = obj.name.Replace(findString, replaceString);
                AssetDatabase.RenameAsset(path, newName);
            }
            AssetDatabase.Refresh();
        }

        private static void RenameSelectedGameObjects(string findString, string replaceString)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.name = obj.name.Replace(findString, replaceString);
            }
        }
    }

}