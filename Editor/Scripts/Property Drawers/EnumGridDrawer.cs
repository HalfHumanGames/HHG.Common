using UnityEditor;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [CustomPropertyDrawer(typeof(EnumGrid), true)]
    public class EnumGridDrawer : PropertyDrawer
    {
        private const int CellSize = 25;
        private const int CellPadding = 2;
        private GUIStyle _invisibleStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize();

            EditorGUI.BeginProperty(position, label, property);

            DrawLabel(ref position, label);

            SerializedProperty gridProperty = property.FindPropertyRelative("grid");
            SerializedProperty sizeProperty = property.FindPropertyRelative("size");
            if (gridProperty == null || sizeProperty == null)
            {
                EditorGUI.LabelField(position, "Grid property not found");
                EditorGUI.EndProperty();
                return;
            }

            EnumGrid enumGrid = (EnumGrid)fieldInfo.GetValue(property.serializedObject.targetObject);
            if (enumGrid == null)
            {
                EditorGUI.LabelField(position, "Grid is null");
                EditorGUI.EndProperty();
                return;
            }

            int newSize = DrawGridSizeField(ref position, sizeProperty, enumGrid);
            Object targetObject = property.serializedObject.targetObject;
            DrawGridCells(ref position, targetObject, enumGrid, newSize);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();
        }

        private void Initialize()
        {
            _invisibleStyle ??= new GUIStyle(EditorStyles.popup)
            {
                normal = { background = Texture2D.blackTexture, textColor = new Color(0, 0, 0, 0) },
                focused = { background = Texture2D.blackTexture, textColor = new Color(0, 0, 0, 0) },
                active = { background = Texture2D.blackTexture, textColor = new Color(0, 0, 0, 0) },
                onNormal = { background = Texture2D.blackTexture, textColor = new Color(0, 0, 0, 0) },
                onFocused = { background = Texture2D.blackTexture, textColor = new Color(0, 0, 0, 0) },
                onActive = { background = Texture2D.blackTexture, textColor = new Color(0, 0, 0, 0) }
            };
        }

        private void DrawLabel(ref Rect position, GUIContent label)
        {
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);
            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;
        }

        private int DrawGridSizeField(ref Rect position, SerializedProperty sizeProperty, EnumGrid enumGrid)
        {
            Rect gridSizeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            int size = sizeProperty.intValue;
            int newSize = EditorGUI.IntField(gridSizeRect, size);

            if (!enumGrid.IsInitialized || size != newSize)
            {
                size = newSize;
                Undo.RecordObject(sizeProperty.serializedObject.targetObject, "Change Grid Size");
                enumGrid.Initialize(newSize);
                sizeProperty.intValue = newSize;
            }

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return newSize;
        }

        private void DrawGridCells(ref Rect position, Object targetObject, EnumGrid enumGrid, int size)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Rect rect = new Rect(position.x + x * (CellSize + CellPadding), position.y + y * (CellSize + CellPadding), CellSize, CellSize);
                    Rect paddedRect = new Rect(rect.x + CellPadding, rect.y + CellPadding, rect.width - 2 * CellPadding, rect.height - 2 * CellPadding);

                    int value = enumGrid.GetCellWeak(x, y);
                    Color color = enumGrid.GetColorWeak(value);

                    EditorGUI.DrawRect(paddedRect, color);

                    System.Type enumType = enumGrid.GetType().BaseType.GetGenericArguments()[0];
                    int newValue = System.Convert.ToInt32(EditorGUI.EnumPopup(rect, (System.Enum)System.Enum.ToObject(enumType, value), _invisibleStyle));

                    if (value != newValue)
                    {
                        Undo.RecordObject(targetObject, "Edit Grid Cell");
                        enumGrid.SetCellWeak(x, y, newValue);
                        EditorUtility.SetDirty(targetObject);
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var sizeProperty = property.FindPropertyRelative("size");
            int size = sizeProperty != null ? sizeProperty.intValue : 0;
            return EditorGUIUtility.singleLineHeight * 3 + (CellSize + CellPadding) * size;
        }
    }
}
