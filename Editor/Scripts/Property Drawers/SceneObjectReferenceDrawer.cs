using HHG.Common.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    [CustomPropertyDrawer(typeof(SceneObjectRegistryAttribute))]
    public class SceneObjectReferenceDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, Dictionary<string, string>> nameToIdMap = new Dictionary<string, Dictionary<string, string>>();
        private static readonly Dictionary<string, Dictionary<string, string>> idToNameMap = new Dictionary<string, Dictionary<string, string>>();
        private static readonly Dictionary<string, GameObject> registryMap = new Dictionary<string, GameObject>();
        private static readonly Dictionary<string, double> lastRefreshMap = new Dictionary<string, double>();
        private const float refreshInterval = 5f;

        private static void RefreshNames(string registryName)
        {
            lastRefreshMap[registryName] = EditorApplication.timeSinceStartup;

            Dictionary<string, string> nameToId = new Dictionary<string, string>();
            Dictionary<string, string> idToName = new Dictionary<string, string>();
            nameToIdMap[registryName] = nameToId;
            idToNameMap[registryName] = idToName;

            GameObject registry = GameObject.Find(registryName);
            registryMap[registryName] = registry;

            if (registry == null)
            {
                return;
            }

            foreach (Transform child in registry.transform)
            {
                SceneObjectId sceneObjectId = child.GetComponent<SceneObjectId>();
                if (sceneObjectId != null && !string.IsNullOrEmpty(sceneObjectId.Id))
                {
                    nameToId[child.name] = sceneObjectId.Id;
                    idToName[sceneObjectId.Id] = child.name;
                }
            }
        }

        private static bool NeedsRefresh(string registryName)
        {
            return !nameToIdMap.ContainsKey(registryName) ||
                !lastRefreshMap.TryGetValue(registryName, out double lastRefreshTime) ||
                EditorApplication.timeSinceStartup - lastRefreshTime > refreshInterval;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SceneObjectRegistryAttribute attr = fieldInfo.GetCustomAttribute<SceneObjectRegistryAttribute>();

            if (attr == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            string registryName = attr.RegistryName;

            if (string.IsNullOrEmpty(registryName))
            {
                registryName = property.serializedObject.FindProperty("registryName")?.stringValue;
            }

            if (string.IsNullOrEmpty(registryName))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (NeedsRefresh(registryName))
            {
                RefreshNames(registryName);
            }

            SerializedProperty stringProp = property.propertyType == SerializedPropertyType.String
                ? property
                : property.FindPropertyRelative("id");

            if (stringProp == null)
            {
                return;
            }

            // Hide label since it shows as the literal guid string
            //position = EditorGUI.PrefixLabel(position, label);
            DrawDropdown(position, stringProp, registryName);
        }

        private static void DrawDropdown(Rect rect, SerializedProperty property, string registryName)
        {
            string currentId = property.stringValue;
            registryMap.TryGetValue(registryName, out GameObject cachedRegistry);
            idToNameMap.TryGetValue(registryName, out Dictionary<string, string> idToName);
            nameToIdMap.TryGetValue(registryName, out Dictionary<string, string> nameToId);
            bool registryMissing = cachedRegistry == null;
            bool isEmpty = string.IsNullOrEmpty(currentId);
            bool isValid = registryMissing || isEmpty || (idToName != null && idToName.ContainsKey(currentId));

            Color prevBg = GUI.backgroundColor;
            if (!isValid)
            {
                GUI.backgroundColor = Color.red;
            }
            else if (registryMissing)
            {
                GUI.backgroundColor = Color.yellow;
            }

            string displayText;
            if (isEmpty)
            {
                displayText = "(none)";
            }
            else if (!registryMissing && idToName != null && idToName.TryGetValue(currentId, out string name))
            {
                displayText = name;
            }
            else
            {
                displayText = "(unknown id)";
            }

            string tooltip = registryMissing ? $"No GameObject named \"{registryName}\" found in the scene." : string.Empty;

            if (GUI.Button(rect, new GUIContent(displayText, tooltip), EditorStyles.popup))
            {
                if (registryMissing)
                {
                    Debug.LogWarning($"[SceneObjectRegistry] No GameObject named \"{registryName}\" found in the scene. Open a scene that contains one to use the dropdown.");
                }
                else
                {
                    ShowEffectMenu(property.Copy(), nameToId);
                }
            }

            GUI.backgroundColor = prevBg;
        }

        private static void ShowEffectMenu(SerializedProperty property, Dictionary<string, string> nameToId)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("(none)"), string.IsNullOrEmpty(property.stringValue), () =>
            {
                property.stringValue = string.Empty;
                property.serializedObject.ApplyModifiedProperties();
            });

            menu.AddSeparator(string.Empty);

            if (nameToId != null)
            {
                foreach (string name in nameToId.Keys.OrderBy(n => n))
                {
                    string capturedId = nameToId[name];
                    string menuPath = name.Replace(" - ", "/");
                    bool isOn = property.stringValue == capturedId;
                    menu.AddItem(new GUIContent(menuPath), isOn, () =>
                    {
                        property.stringValue = capturedId;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }

            menu.ShowAsContext();
        }
    }
}
