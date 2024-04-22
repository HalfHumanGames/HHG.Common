using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    public class RenameChildrenTool : EditorWindow
    {
        private string renameString;

        [MenuItem("GameObject/Tools/Rename Children", false, 0)]
        public static void ShowRenameChildrenWindow()
        {
            GetWindow<RenameChildrenTool>(true, "Rename Assets", true);
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Rename children game objects.", MessageType.None);
            renameString = EditorGUILayout.TextField("Name", renameString);
            if (GUILayout.Button("Rename"))
            {
                RenameSelectedAssets(renameString);
                Close();
            }
        }

        private static void RenameSelectedAssets(string renameString)
        {
            Transform parent = ((GameObject)Selection.activeObject).transform;
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                parent.GetChild(i).name = renameString + "_" + i;
            }
        }
    } 
}
