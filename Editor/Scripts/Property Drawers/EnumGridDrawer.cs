using UnityEditor;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [CustomPropertyDrawer(typeof(EnumGrid), true)]
    public class EnumGridDrawer : PropertyDrawer
    {
        private const int cellSize = 20;
        private const int cellPadding = 1;

        private GUIStyle invisibleStyle;
        private GUIStyleState invisibleStyleState;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize();

            EditorGUI.BeginProperty(position, label, property);

            DrawLabel(ref position, label);
            DrawGridSizeField(ref position, property);
            DrawGridCells(ref position, property);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();
        }

        private void Initialize()
        {
            invisibleStyleState ??= new GUIStyleState
            {
                background = Texture2D.blackTexture,
                textColor = Color.clear
            };

            invisibleStyle ??= new GUIStyle(EditorStyles.popup)
            {
                normal = invisibleStyleState,
                focused = invisibleStyleState,
                active = invisibleStyleState,
                onNormal = invisibleStyleState,
                onFocused = invisibleStyleState,
                onActive = invisibleStyleState
            };
        }

        private void DrawLabel(ref Rect position, GUIContent label)
        {
            Rect rect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, label);
            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;
        }

        private void DrawGridSizeField(ref Rect position, SerializedProperty property)
        {
            SerializedProperty gridProperty = property.FindPropertyRelative("grid");
            SerializedProperty sizeProperty = property.FindPropertyRelative("size");

            EnumGrid enumGrid = property.boxedValue as EnumGrid;

            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            int size = sizeProperty.intValue;
            int newSize = EditorGUI.IntField(rect, size);

            if (!enumGrid.IsInitialized || size != newSize)
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Change Grid Size");
                enumGrid.Initialize(newSize);
                property.boxedValue = enumGrid;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void DrawGridCells(ref Rect position, SerializedProperty property)
        {
            SerializedProperty gridProperty = property.FindPropertyRelative("grid");
            SerializedProperty sizeProperty = property.FindPropertyRelative("size");

            int size = sizeProperty.intValue;
            EnumGrid enumGrid = property.boxedValue as EnumGrid;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Rect rect = new Rect(position.x + x * (cellSize + cellPadding), position.y + y * (cellSize + cellPadding), cellSize, cellSize);
                    Rect paddedRect = new Rect(rect.x + cellPadding, rect.y + cellPadding, rect.width - 2 * cellPadding, rect.height - 2 * cellPadding);

                    int value = enumGrid.GetCellWeak(new Vector3Int(x, y));
                    Color color = enumGrid.GetColorWeak(value);

                    EditorGUI.DrawRect(paddedRect, color);

                    System.Type enumType = enumGrid.GetType().BaseType.GetGenericArguments()[0];
                    int newValue = System.Convert.ToInt32(EditorGUI.EnumPopup(rect, (System.Enum)System.Enum.ToObject(enumType, value), invisibleStyle));

                    if (value != newValue)
                    {
                        Undo.RecordObject(property.serializedObject.targetObject, "Edit Grid Cell");
                        enumGrid.SetCellWeak(new Vector3Int(x, y), newValue);
                        property.boxedValue = enumGrid;
                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty sizeProperty = property.FindPropertyRelative("size");
            int size = sizeProperty != null ? sizeProperty.intValue : 0;
            return EditorGUIUtility.singleLineHeight + (cellSize + cellPadding) * size;
        }
    }
}
