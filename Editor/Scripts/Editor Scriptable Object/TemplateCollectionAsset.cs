using System.Collections.Generic;
using System.Linq;
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

        private HashSet<string> currentPaths = new HashSet<string>();
        private HashSet<string> previousPaths = new HashSet<string>();
        
        private void Awake()
        {
            EditorApplication.delayCall += SetupMenuItems;
        }

        protected override sealed void SetupMenuItems()
        {
            if (this == null)
            {
                return;
            }

            templates.Sort((a, b) => a.name.CompareTo(b.name));

            foreach (T template in templates)
            {
                string path = $"{this.path}/{template.name}";
                bool exists = MenuTool.MenuItemExists(path);

                if (!exists && enabled)
                {
                    currentPaths.Add(path);
                    MenuTool.AddMenuItem(path, string.Empty, false, 0, () => Create(template), () => Validate(template));
                }
                else if (exists && !enabled)
                {
                    currentPaths.Remove(path);
                    MenuTool.RemoveMenuItem(path);
                }
            }

            foreach (string path in previousPaths.Except(currentPaths))
            {
                MenuTool.RemoveMenuItem(path);
            }

            previousPaths.Clear();
            previousPaths.UnionWith(currentPaths);
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

            SetupMenuItems();
        }
    }
}