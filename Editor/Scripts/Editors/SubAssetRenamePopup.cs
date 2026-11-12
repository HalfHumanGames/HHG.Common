using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    // Unity can't rename sub-assets from the Project window, so this small window does it
    public class SubAssetRenamePopup : EditorWindow
    {
        private const string fieldName = "SubAssetName";

        private ScriptableObject subAsset;
        private string newName;
        private bool focused;

        public static void Open(ScriptableObject subAsset)
        {
            SubAssetRenamePopup popup = CreateInstance<SubAssetRenamePopup>();
            popup.subAsset = subAsset;
            popup.newName = subAsset.name;
            popup.titleContent = new GUIContent("Rename Sub-Asset");
            popup.minSize = popup.maxSize = new Vector2(300f, 70f);
            popup.ShowUtility();
        }

        private void OnGUI()
        {
            if (subAsset == null)
            {
                Close();
                return;
            }

            bool submit = Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
            bool cancel = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;

            GUI.SetNextControlName(fieldName);
            newName = EditorGUILayout.TextField("Name", newName);

            if (!focused)
            {
                EditorGUI.FocusTextInControl(fieldName);
                focused = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            cancel |= GUILayout.Button("Cancel", GUILayout.Width(80f));
            submit |= GUILayout.Button("Rename", GUILayout.Width(80f));
            EditorGUILayout.EndHorizontal();

            if (submit)
            {
                SubAssetUtil.Rename(subAsset, newName);
                Close();
            }
            else if (cancel)
            {
                Close();
            }
        }
    }
}
