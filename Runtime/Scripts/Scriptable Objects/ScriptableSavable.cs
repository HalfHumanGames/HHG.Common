using System.Diagnostics;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract class ScriptableSavable : ScriptableObject, IUpdated
    {
        private const string extension = ".sav";

        public System.DateTime LastSaved => lastSaved.Value;
        public string CurrentFileName => currentFileName;

        public virtual string DefaultFileName => nameof(ScriptableSavable);

        [SerializeReference, SubclassSelector] private IIO io = new FileIO();
        [SerializeReference, SubclassSelector] private ISerializer serializer = new JsonSerializer();

        [SerializeField, HideInInspector] private SerializedDateTime lastSaved;

        private string currentFileName;

        [ContextMenu(nameof(Save))] public void Save() => Save(currentFileName);
        [ContextMenu(nameof(Load))] public void Load() => Load(currentFileName);
        [ContextMenu(nameof(Delete))] public void Delete() => Delete(currentFileName);

        [ContextMenu("Open in Explorer", false, int.MaxValue)]
        private void OpenInExplorer()
        {
            Process.Start("explorer.exe", Application.persistentDataPath.Replace("/", "\\"));
        }

        private bool loaded;

        public event System.Action Updated;

        private void Awake()
        {
            currentFileName = DefaultFileName;
        }

        public virtual void Reset() { }

        protected virtual void OnUpdated() { }

        public void Save(string fileName)
        {
            string path = GetFileNameWithExtension(fileName);
            byte[] bytes = serializer.Serialize(this);
            io.WriteAllBytes(path, bytes);
            lastSaved.Value = System.DateTime.UtcNow;
        }

        public bool LoadLastSaved(params string[] fileNames)
        {
            string path = GetLastSavedFileName(fileNames);
            
            if (!string.IsNullOrEmpty(path))
            {
                Load(path);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Load(string fileName)
        {
            if (Exists(fileName))
            {
                string path = GetFileNameWithExtension(fileName);
                byte[] bytes = io.ReadAllBytes(path);
                serializer.DeserializeOverwrite(bytes, this);
            }
            else
            {
                Reset();
            }

            currentFileName = fileName;
            loaded = true;
        }

        public bool Exists(string fileName)
        {
            return io.Exists(GetFileNameWithExtension(fileName));
        }

        public bool AnyExists(params string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                if (Exists(fileName))
                {
                    return true;
                }
            }

            return false;
        }

        public bool AllExists(params string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                if (!Exists(fileName))
                {
                    return false;
                }
            }

            return true;
        }

        public void Delete(string fileName)
        {
            string path = GetFileNameWithExtension(fileName);

            if (io.Exists(path))
            {
                io.Delete(path);
            }

            if (currentFileName == fileName)
            {
                Reset();
            }
        }

        protected T Get<T>(ref T value)
        {
            if (!loaded)
            {
                Load();
            }

            return value;
        }

        protected void Set<T>(ref T value, T newValue)
        {
            value = newValue;
            OnUpdated();
            InvokeUpdated();
        }

        protected void InvokeUpdated()
        {
            Updated?.Invoke();
        }

        private string GetFileNameWithExtension(string fileName)
        {
            return $"{fileName}{extension}";
        }

        private string GetLastSavedFileName(params string[] fileNames)
        {
            string lastSavedFileName = string.Empty;
            System.DateTime dateTime = new System.DateTime();

            foreach (string fileName in fileNames)
            {
                if (Exists(fileName))
                {
                    Load(fileName);

                    if (LastSaved > dateTime)
                    {
                        lastSavedFileName = fileName;
                        dateTime = LastSaved;
                    }
                }
            }

            return lastSavedFileName;
        }
    }
}