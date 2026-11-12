using HHG.Common.Runtime;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    // Hides the raw list: shows the count and an Add button. Everything else is in the Project window's Tools > Sub-Assets menu.
    [CustomPropertyDrawer(typeof(SubAssetList<>))]
    public class SubAssetListDrawer : PropertyDrawer
    {
        private const float buttonWidth = 50f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int count = property.FindPropertyRelative("items").arraySize;
            Rect labelRect = new Rect(position.x, position.y, position.width - buttonWidth - 4f, position.height);
            Rect buttonRect = new Rect(position.xMax - buttonWidth, position.y, buttonWidth, position.height);

            EditorGUI.LabelField(labelRect, label, new GUIContent($"{count} sub-asset{(count == 1 ? "" : "s")}"));

            if (GUI.Button(buttonRect, "Add"))
            {
                Object owner = property.serializedObject.targetObject;
                string fieldName = property.propertyPath;
                System.Type type = fieldInfo.FieldType.GetGenericArguments()[0];

                // Defer so asset changes don't happen mid-inspector draw
                EditorApplication.delayCall += () => SubAssetUtil.Add(owner, fieldName, type);
            }
        }
    }
}
