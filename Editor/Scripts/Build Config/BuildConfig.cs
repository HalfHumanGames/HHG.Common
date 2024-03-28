using HHG.Common.Runtime;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Editor
{
    // Don't use Path.Combine since it doesn't work on Mac and Linux
    // Use forward slash which works on all platforms including Windows
    public class BuildConfig : EditorScriptableObject<BuildConfig>
    {
        private static readonly string[] exclude = new string[] { "DoNotShip", "DontShip" };

        [SerializeField] private string BuildFolderPath;
        [SerializeField] private BuildConfigOptions[] options;

        [ContextMenu("Build")]
        private void Build()
        {
            if (SceneManager.GetActiveScene().isDirty)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    if (SceneManager.GetActiveScene().isDirty)
                    {
                        Debug.LogWarning("Save or discard scene changes before building.");
                    }
                    else
                    {
                        BuildInternal();
                    }
                }
            }
            else
            {
                BuildInternal();
            }
        }

        private void BuildInternal()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;
            ScriptingImplementation backend = PlayerSettings.GetScriptingBackend(group);

            for (int i = 0; i < options.Length; i++)
            {
                if (options[i].Enabled)
                {
                    PlayerSettings.SetScriptingBackend(BuildPipeline.GetBuildTargetGroup(options[i].Target), options[i].Backend);

                    string folder = $"{BuildFolderPath}/{options[i].Target.GetDisplayName()}";
                    string filename = $"{folder}/{Application.productName}.{options[i].Target.GetExtension()}";
                    BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, filename, options[i].Target, options[i].Options);
                    BuildSummary summary = report.summary;

                    if (summary.result == BuildResult.Succeeded)
                    {
                        string project = Application.dataPath.Replace("Assets", string.Empty);
                        string source = $"{project}{folder}/";
                        string destination = $"{project}{BuildFolderPath}/{Application.productName}-{options[i].Target.GetDisplayName()}.zip";

                        if (File.Exists(destination))
                        {
                            File.Delete(destination);
                        }

                        using (FileStream stream = new FileStream(destination, FileMode.Create))
                        using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Create))
                        {                
                            
                            List<ZipArchiveEntry> entries = new List<ZipArchiveEntry>();
                            IEnumerable<string> paths = Directory.GetFiles(source).Concat(Directory.GetDirectories(source)).Where(p => exclude.All(e => !p.Contains(e)));

                            foreach (string path in paths)
                            {
                                zip.CreateEntryFrom(path, string.Empty);
                            }

                            foreach(ZipArchiveEntry entry in entries)
                            {
                                // Make sure to set the execution permissions for mac and linux so the app can run
                                entry.ExternalAttributes = UnixUtility.GetUnixPermissions(755);
                            }
                        }

                        Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                    }

                    if (summary.result == BuildResult.Failed)
                    {
                        Debug.LogError("Build failed!");

                        foreach (BuildStep step in report.steps)
                        {
                            foreach (BuildStepMessage message in step.messages)
                            {
                                if (message.type == LogType.Error || message.type == LogType.Exception)
                                {
                                    Debug.LogError($"Build error: {message.content}");
                                }
                            }
                        }

                        break;
                    }
                }
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, backend);
            OpenBuildFolder();
        }

        [ContextMenu("Open Build Folder")]
        private void OpenBuildFolder()
        {
            string path = Application.dataPath.Replace("Assets", BuildFolderPath);
            EditorUtility.RevealInFinder(path);
        }

        [MenuItem("Half Human Games/Build Config")]
        public static void Open()
        {
            Selection.activeObject = LoadOrCreateInstance();
        }
    }
}
