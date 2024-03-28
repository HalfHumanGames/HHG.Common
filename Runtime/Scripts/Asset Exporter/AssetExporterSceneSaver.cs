#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    [InitializeOnLoad]
    public static class AssetExporterSceneSaver
    {
        static AssetExporterSceneSaver()
        {
            EditorApplication.delayCall += OnDelayCall;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorSceneManager.sceneSaved += OnSceneSaved;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeDomainReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterDomainReload;
        }

        // EditorSceneManager.sceneOpened does not get called when Unity opens for the
        // first time. Nor is the scene loaded when the static constructor gets called.
        // EditorApplication.delayCall ensures it gets called after the scene is loaded.
        private static void OnDelayCall()
        {
            EditorApplication.delayCall -= OnDelayCall;

            if (Application.isPlaying) return;

            foreach (AssetExporterBase exporter in FindExporters())
            {
                exporter.Load();
            }
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (CanContinue())
            {
                foreach (AssetExporterBase exporter in FindExporters())
                {
                    exporter.Load();
                }
            }
        }

        private static void OnSceneSaving(Scene scene, string path)
        {
            if (CanContinue())
            {
                foreach (AssetExporterBase exporter in FindExporters())
                {
                    exporter.Save();
                    exporter.Clear();
                }
            }
        }

        private static void OnSceneSaved(Scene scene)
        {
            if (CanContinue())
            {
                foreach (AssetExporterBase exporter in FindExporters())
                {
                    exporter.Load();
                }
            }
        }

        private static void OnBeforeDomainReload()
        {
            if (CanContinue())
            {
                foreach (AssetExporterBase exporter in FindExporters())
                {
                    EditorPrefs.SetString(exporter.Id, exporter.ToJson());
                }
            }
        }

        private static void OnAfterDomainReload()
        {
            if (CanContinue())
            {
                foreach (AssetExporterBase exporter in FindExporters())
                {
                    if (EditorPrefs.HasKey(exporter.Id))
                    {
                        exporter.FromJson(EditorPrefs.GetString(exporter.Id));

                        EditorPrefs.DeleteKey(exporter.Id);
                    }
                    else
                    {
                        exporter.Load();
                    }
                }
            }
        }

        private static bool CanContinue()
        {
            return !BuildPipeline.isBuildingPlayer;
        }

        private static IEnumerable<AssetExporterBase> FindExporters()
        {
            return Object.FindObjectsOfType<AssetExporterBase>().Where(e => e.HasAsset);
        }
    }
}
#endif