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
            GuidAttribute guid = (GuidAttribute)attribute;

            if (string.IsNullOrEmpty(guid.Path))
            {
                string assetPath = AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
                assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            }
            else
            {
                SerializedProperty parentProperty = property.GetParentProperty();
                SerializedProperty targetProperty = parentProperty != null ?
                    parentProperty.FindPropertyRelative(guid.Path):
                    property.serializedObject.FindProperty(guid.Path);

                if (targetProperty != null && targetProperty.propertyType == SerializedPropertyType.ObjectReference)
                {
                    Object targetObject = targetProperty.objectReferenceValue;

                    if (targetObject != null)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(targetObject);
                        assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
                    }
                }
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
