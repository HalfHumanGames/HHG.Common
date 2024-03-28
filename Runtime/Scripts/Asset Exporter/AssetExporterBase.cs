using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HHG.Common
{
    public abstract class AssetExporterBase : MonoBehaviour
    {
        public string Id => id;
        public abstract bool HasAsset { get; }

        [SerializeField, HideInInspector] private string id => Guid.NewGuid().ToString();

        [ContextMenu("Load Asset")]
        public void Load() => LoadInternal();

        [ContextMenu("Load Asset From...")]
        public void LoadFrom() => LoadFromInternal();

        [ContextMenu("Save Asset")]
        public void Save() => SaveInternal();

        [ContextMenu("Save Asset As...")]
        public void SaveAs() => SaveAsInternal();

        public void Clear() => ClearInternal();

        protected abstract void LoadInternal();
        protected abstract void LoadFromInternal();
        protected abstract void LoadFromInternal(string path);
        protected abstract void SaveInternal();
        protected abstract void SaveAsInternal();
        protected abstract void SaveAsInternal(string path);
        protected abstract void ClearInternal();

        public abstract string ToJson();
        public abstract void FromJson(string json);
    }

    public abstract class AssetExporterBase<TAsset> : AssetExporterBase where TAsset : ScriptableObject
    {
        public bool LoadOnAwake => Application.isEditor ? loadOnAwakeEditor : loadOnAwakePlayer;
        public string Path => path;
        public TAsset Current
        {
            get => current;
            set
            {
                if (current != value)
                {
                    current = value;
                    Load();
                }
            }
        }
        public sealed override bool HasAsset => current != null;
        public bool IsLoaded => isLoaded;
        public UnityEvent Loaded;

        [SerializeField] private bool loadOnAwakeEditor;
        [SerializeField] private bool loadOnAwakePlayer;
        [SerializeField] private string path;
        [SerializeField] private TAsset current;

        private bool isLoaded;

        private void Awake()
        {
            if (LoadOnAwake)
            {
                Load();

                // For some reason, colliders don't work 
                // unless we toggle the game object off/on
                gameObject.SetActive(false);
                gameObject.SetActive(true);
            }
        }

        protected virtual void Save(TAsset asset) { }
        protected virtual void Load(TAsset asset) { }
        protected virtual void Clear(TAsset asset) { }

        public sealed override string ToJson()
        {
            TAsset temp = ScriptableObject.CreateInstance<TAsset>();
            Save(temp);
            string json = JsonUtility.ToJson(temp);
            DestroyImmediate(temp);
            return json;
        }

        public sealed override void FromJson(string json)
        {
            TAsset temp = ScriptableObject.CreateInstance<TAsset>();
            JsonUtility.FromJsonOverwrite(json, temp);
            Load(temp);
            DestroyImmediate(temp);
        }

        protected sealed override void LoadInternal()
        {
            Load(current);
            isLoaded = true;
            Loaded?.Invoke();
        }

#if UNITY_EDITOR

        protected sealed override void LoadFromInternal()
        {
            // There is no InProject version that returns a relative path, so
            // we call Substring since OpenFilePanel returns an absolute path
            // Make sure to also subtract 6 to include "Assets" to the path
            string assetPath = EditorUtility.OpenFilePanel("Save Scriptable Object", path, "asset").Substring(Application.dataPath.Length - 6);

            if (!string.IsNullOrEmpty(assetPath))
            {
                LoadFromInternal(assetPath);
            }
            else
            {
                Debug.LogWarning("Open operation canceled.");
            }
        }

        protected sealed override void LoadFromInternal(string assetPath)
        {
            current = AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);
            Load(current);
        }

        protected sealed override void SaveInternal()
        {
            if (current != null)
            {
                Save(current);
                EditorUtility.SetDirty(current);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        protected sealed override void SaveAsInternal()
        {
            string assetPath = EditorUtility.SaveFilePanelInProject("Save Scriptable Object", "New Tilemap", "asset", "", path);

            if (!string.IsNullOrEmpty(assetPath))
            {
                SaveAsInternal(assetPath);
            }
            else
            {
                Debug.LogWarning("Save operation canceled.");
            }
        }

        protected sealed override void SaveAsInternal(string assetPath)
        {
            current = ScriptableObject.CreateInstance<TAsset>();
            Save(current);
            AssetDatabase.CreateAsset(current, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

#else

        protected override void LoadFromInternal() { }
        protected override void LoadFromInternal(string path) { }
        protected override void SaveInternal() { }
        protected override void SaveAsInternal() { }
        protected override void SaveAsInternal(string path) { }

#endif

        protected sealed override void ClearInternal()
        {
            Clear(current);
        }
    }
}