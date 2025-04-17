using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    public class FindReferencesTool : EditorWindow
    {
        private string guidToFind = string.Empty;
        private string replacementGuid = string.Empty;
        private Object searchedObject;
        private Dictionary<Object, int> referenceObjects = new Dictionary<Object, int>();
        private Vector2 scrollPosition;
        private System.Diagnostics.Stopwatch searchTimer = new System.Diagnostics.Stopwatch();

        [MenuItem("| Half Human Games |/Tools/Reference Finder")]
        private static void Open()
        {
            GetWindow(typeof(FindReferencesTool), false, "Reference Finder");
        }

        private void OnGUI()
        {
            if (EditorSettings.serializationMode == SerializationMode.ForceText)
            {
                DisplayMainMenu();

                if (GUILayout.Button("Search"))
                {
                    searchTimer.Reset();
                    searchTimer.Start();
                    referenceObjects.Clear();

                    string pathToAsset = AssetDatabase.GUIDToAssetPath(guidToFind);

                    if (!string.IsNullOrEmpty(pathToAsset))
                    {
                        searchedObject = AssetDatabase.LoadAssetAtPath<Object>(pathToAsset);

                        List<string> allPathToAssetsList = new List<string>();
                        string[] allPrefabs = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
                        allPathToAssetsList.AddRange(allPrefabs);
                        string[] allMaterials = Directory.GetFiles(Application.dataPath, "*.mat", SearchOption.AllDirectories);
                        allPathToAssetsList.AddRange(allMaterials);
                        string[] allScenes = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);
                        allPathToAssetsList.AddRange(allScenes);
                        string[] allControllers = Directory.GetFiles(Application.dataPath, "*.controller", SearchOption.AllDirectories);
                        allPathToAssetsList.AddRange(allControllers);
                        string[] allVfxGraphs = Directory.GetFiles(Application.dataPath, "*.vfx", SearchOption.AllDirectories);
                        allPathToAssetsList.AddRange(allVfxGraphs);
                        string[] allShaderGraphs = Directory.GetFiles(Application.dataPath, "*.shadergraph", SearchOption.AllDirectories);
                        allPathToAssetsList.AddRange(allShaderGraphs);
                        string[] allAssets = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);
                        allPathToAssetsList.AddRange(allAssets);

                        for (int i = 0; i < allPathToAssetsList.Count; i++)
                        {
                            string assetPath = allPathToAssetsList[i];
                            string text = File.ReadAllText(assetPath);
                            string[] lines = text.Split('\n');

                            for (int j = 0; j < lines.Length; j++)
                            {
                                string line = lines[j];

                                if (line.Contains("guid:"))
                                {
                                    if (line.Contains(guidToFind))
                                    {
                                        string pathToReferenceAsset = assetPath.Replace(Application.dataPath, string.Empty);
                                        pathToReferenceAsset = pathToReferenceAsset.Replace(".meta", string.Empty);
                                        string path = "Assets" + pathToReferenceAsset;
                                        path = path.Replace(@"\", "/"); // fix OSX/Windows path

                                        Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                                        if (asset != null)
                                        {
                                            if (!referenceObjects.ContainsKey(asset))
                                            {
                                                referenceObjects.Add(asset, 0);
                                            }

                                            referenceObjects[asset]++;
                                        }
                                        else
                                        {
                                            Debug.LogError(path + " could not be loaded");
                                        }
                                    }
                                }
                            }
                        }

                        searchTimer.Stop();
                    }
                    else
                    {
                        Debug.LogError("no asset found for GUID: " + guidToFind);
                    }
                }

                if (referenceObjects.Count > 0 && GUILayout.Button("Replace"))
                {
                    ReplaceGuids(referenceObjects, guidToFind, replacementGuid);
                }

                DisplayReferenceObjectList(referenceObjects);
            }
            else
            {
                DisplaySerializationWarning();
            }
        }

        private void ReplaceGuids(Dictionary<Object, int> referenceObjects, string guidToFind, string replacementGuid)
        {
            foreach (Object referenceObject in referenceObjects.Keys)
            {
                string assetPath = AssetDatabase.GetAssetPath(referenceObject);
                string text = File.ReadAllText(assetPath);
                string newText = text.Replace(guidToFind, replacementGuid);
                Debug.Log("Overwriting file data of: " + referenceObject.name + "\n\nOld:\n" + text + "\n\nNew:\n" + newText);
                File.WriteAllText(assetPath, newText);
            }

            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }

        private void DisplaySerializationWarning()
        {
            GUILayout.Label("The Reference Finder relies on readable meta files (visible text serialization).\nPlease change your serialization mode in \"Edit/Project Settings/Editor/Version Control\"\n to \"Visisble Meta Files\" and \"Asset Serialization\" to \"Force Text\".");
        }

        private void DisplayReferenceObjectList(Dictionary<Object, int> referenceObjectsDictionary)
        {
            GUILayout.Label("Reference by: " + referenceObjectsDictionary.Count + " assets. (Last search duration: " + searchTimer.Elapsed + ")");

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var kvpair in referenceObjectsDictionary)
            {
                Object referencingObject = kvpair.Key;
                int referenceCount = kvpair.Value;
                EditorGUILayout.ObjectField(referencingObject.name + " (" + referenceCount + ")", referencingObject, typeof(Object), false);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DisplayMainMenu()
        {
            EditorGUILayout.BeginHorizontal();

            searchedObject = EditorGUILayout.ObjectField(searchedObject != null ? searchedObject.name : "Drag & Drop Asset", searchedObject, typeof(Object), false);
            if (GUILayout.Button("Get GUID") && searchedObject != null)
            {
                string pathToAsset = AssetDatabase.GetAssetPath(searchedObject);
                guidToFind = AssetDatabase.AssetPathToGUID(pathToAsset);
            }

            EditorGUILayout.EndHorizontal();

            string newGuidToFind = EditorGUILayout.TextField("GUID", guidToFind);
            if (!guidToFind.Equals(newGuidToFind))
            {
                guidToFind = newGuidToFind;
            }

            if (referenceObjects != null && referenceObjects.Count > 0)
            {
                string newReplacementGuid = EditorGUILayout.TextField("Replacement", replacementGuid);
                if (!replacementGuid.Equals(newReplacementGuid))
                {
                    replacementGuid = newReplacementGuid;
                }
            }
        }
    }
}