using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HHG.Common.Editor
{
    public abstract class TemplateCollectionAsset : ScriptableObject
    {
        protected abstract string defaultPath { get; }

        protected abstract void SetupMenuItems();

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            foreach (TemplateCollectionAsset templateCollection in AssetDatabaseUtil.FindAssets<TemplateCollectionAsset>())
            {
                templateCollection.SetupMenuItems();
            }
        }
    }

    public abstract class TemplateCollectionAsset<T> : TemplateCollectionAsset where T : Object
    {
        [SerializeField] private bool enabled = true;
        [SerializeField] private string path;
        [SerializeField] private List<T> templates = new List<T>();
        [SerializeField, HideInInspector] private string previousPath;

        private void Awake()
        {
            EditorApplication.delayCall += SetupMenuItems;
        }

        protected override sealed void SetupMenuItems()
        {
            foreach (T template in templates)
            {
                string fullPath = $"{path}/{template.name}";
                string previousFullPath = $"{previousPath}/{template.name}";
                bool exists = MenuTool.MenuItemExists(fullPath);

                if (fullPath != previousFullPath)
                {
                    MenuTool.RemoveMenuItem(previousFullPath);
                }

                if (!exists && enabled)
                {
                    MenuTool.AddMenuItem(fullPath, string.Empty, false, 0, () => Create(template), () => Validate(template));
                }
                else if (exists && !enabled)
                {
                    MenuTool.RemoveMenuItem(fullPath);
                }

            }

            previousPath = path;
        }

        public static void Create<TCollection>(T asset) where TCollection : TemplateCollectionAsset<T>
        {
            TCollection instance = CreateInstance<TCollection>();
            instance.Create(asset);
            DestroyImmediate(instance);
        }

        protected abstract void Create(T template);

        protected virtual bool Validate(T template) => true;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(path))
            {
                path = defaultPath;
            }

            EditorApplication.delayCall += SetupMenuItems;
        }
    }
}