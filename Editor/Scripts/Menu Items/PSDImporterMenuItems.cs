using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.PSD;
using UnityEngine;

namespace HHG.Common.Editor
{
    public static class PSDImporterMenuItems
    {
        [MenuItem("| Half Human Games |/Tools/PSD/Enables Layers")]
        private static void EnableLayersMenuItem()
        {
            string[] selectedPaths = GetSelectedPSBPaths();
            HashSet<string> prefixSet = new HashSet<string>();

            foreach (string path in selectedPaths)
            {
                if (!(AssetImporter.GetAtPath(path) is PSDImporter importer)) continue;

                SerializedObject so = new SerializedObject(importer);
                SerializedProperty psdLayers = so.FindProperty("m_PsdLayers");

                if (psdLayers == null) continue;

                so.Update();

                for (int i = 0; i < psdLayers.arraySize; i++)
                {
                    SerializedProperty layer = psdLayers.GetArrayElementAtIndex(i);
                    SerializedProperty nameProp = layer.FindPropertyRelative("m_Name");
                    SerializedProperty isGroupProp = layer.FindPropertyRelative("m_IsGroup");

                    if (nameProp == null || isGroupProp == null) continue;
                    if (isGroupProp.boolValue) continue;

                    string layerName = nameProp.stringValue;
                    if (string.IsNullOrWhiteSpace(layerName) || layerName.StartsWith("_")) continue;

                    int dashIndex = layerName.IndexOf('-');
                    string prefix = dashIndex > 0 ? layerName.Substring(0, dashIndex).Trim() : layerName.Trim();

                    if (!string.IsNullOrEmpty(prefix)) prefixSet.Add(prefix);
                }
            }

            if (prefixSet.Count == 0)
            {
                Debug.LogWarning("[PSDImporter] No valid prefixes found in selected asset(s).");
                return;
            }

            string[] prefixes = new string[prefixSet.Count];
            prefixSet.CopyTo(prefixes);
            Array.Sort(prefixes, StringComparer.OrdinalIgnoreCase);

            Rect parentRect = EditorWindow.focusedWindow?.position ?? new Rect(0, 0, 800, 600);
            PrefixPickerWindow.Show(prefixes, parentRect, SetLayersByPrefixOnSelection);
        }

        [MenuItem("| Half Human Games |/Tools/PSD/Enables Layers", validate = true)]
        private static bool EnableLayersValidate() => IsValidSelection();

        private static bool IsValidSelection()
        {
            foreach (string path in GetSelectedPSBPaths())
            {
                if (AssetImporter.GetAtPath(path) is PSDImporter)
                {
                    return true;
                }
            }
            return false;
        }

        private static void SetLayersByPrefixOnSelection(string prefix)
        {
            foreach (string path in GetSelectedPSBPaths())
            {
                if (AssetImporter.GetAtPath(path) is PSDImporter importer)
                {
                    SetLayersByPrefix(importer, prefix);
                }
            }
        }

        private static string[] GetSelectedPSBPaths()
        {
            return Array.ConvertAll(Selection.assetGUIDs, AssetDatabase.GUIDToAssetPath);
        }

        private static void SetLayersByPrefix(PSDImporter importer, string prefix)
        {
            SerializedObject so = new SerializedObject(importer);
            SerializedProperty psdLayers = so.FindProperty("m_PsdLayers");
            SerializedProperty importSettings = so.FindProperty("m_PSDLayerImportSetting");

            if (psdLayers == null || importSettings == null)
            {
                Debug.LogError($"[PSDImporter] Could not find required serialized properties on '{importer.assetPath}'.");
                return;
            }

            so.Update();
            importSettings.ClearArray();

            HashSet<int> seen = new HashSet<int>();
            int enabled = 0, disabled = 0;
            for (int i = 0; i < psdLayers.arraySize; i++)
            {
                SerializedProperty layer = psdLayers.GetArrayElementAtIndex(i);
                SerializedProperty nameProp = layer.FindPropertyRelative("m_Name");
                SerializedProperty layerIdProp = layer.FindPropertyRelative("m_LayerID");
                SerializedProperty isGroupProp = layer.FindPropertyRelative("m_IsGroup");

                if (nameProp == null || layerIdProp == null || isGroupProp == null) continue;
                if (isGroupProp.boolValue) continue;

                int layerId = layerIdProp.intValue;
                string layerName = nameProp.stringValue;
                if (!seen.Add(layerId)) continue;

                bool shouldImport = layerName.StartsWith(prefix);

                int idx = importSettings.arraySize;
                importSettings.InsertArrayElementAtIndex(idx);
                SerializedProperty entry = importSettings.GetArrayElementAtIndex(idx);
                entry.FindPropertyRelative("name").stringValue = layerName;
                entry.FindPropertyRelative("layerId").intValue = layerId;
                entry.FindPropertyRelative("flatten").boolValue = false;
                entry.FindPropertyRelative("isGroup").boolValue = false;
                entry.FindPropertyRelative("importLayer").boolValue = shouldImport;

                if (shouldImport) enabled++; else disabled++;
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            Debug.Log($"[PSDImporter] '{prefix}' on '{importer.assetPath}': {enabled} enabled, {disabled} disabled.");
        }

        private class PrefixPickerWindow : EditorWindow
        {
            private string[] prefixes;
            private Action<string> onSelected;

            public static void Show(string[] prefixes, Rect parentRect, Action<string> onSelected)
            {
                PrefixPickerWindow window = CreateInstance<PrefixPickerWindow>();
                window.prefixes = prefixes;
                window.onSelected = onSelected;
                window.titleContent = new GUIContent("Select Prefix");

                float rowHeight = EditorGUIUtility.singleLineHeight + 4f;
                float height = prefixes.Length * rowHeight + 8f;
                float x = parentRect.x + (parentRect.width - 200f) * 0.5f;
                float y = parentRect.y + (parentRect.height - height) * 0.5f;

                window.position = new Rect(x, y, 200f, height);
                window.ShowPopup();
            }

            private void OnGUI()
            {
                foreach (string prefix in prefixes)
                {
                    if (GUILayout.Button(prefix))
                    {
                        onSelected?.Invoke(prefix);
                        Close();
                        return;
                    }
                }

                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                {
                    Close();
                }
            }

            private void OnLostFocus()
            {
                Close();
            }
        }
    }
}
