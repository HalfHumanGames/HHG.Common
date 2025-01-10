using HHG.Common.Runtime;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(GuidAttribute), true)]
    public class GuidDrawer : PropertyDrawer
    {
        private string assetGuid;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (string.IsNullOrEmpty(assetGuid))
            {
                string assetPath = AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
                assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            }

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(position, property, label);

            if (property.propertyType == SerializedPropertyType.String)
            {
                if (property.stringValue != assetGuid)
                {
                    property.stringValue = assetGuid;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
