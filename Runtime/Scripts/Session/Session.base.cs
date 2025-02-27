using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public abstract partial class Session : ScriptableObject, IBindable
    {
        [ContextMenu("Save")] private void _Save() => save();
        [ContextMenu("Load")] private void _Load() => load();
        [ContextMenu("Clear")] private void _Clear() => clear();

        public abstract string fileId { get; set; }
        public abstract string tempFileId { get; }
        public abstract ISessionState readOnlyStateWeak { get; }
        public abstract bool hasStagedChanged { get; }
        public abstract string defaultFileId { get; }
        public abstract string tempFileTag { get; }
        public abstract string fileExtension { get; }
        public abstract string[] fileIds { get; }
        public abstract bool logsEnabled { get; }

        public abstract void initialize();
        public abstract void log(string message);
        public abstract void saveWeak(Action<object> mutation);
        public abstract void stageWeak(Action<object> mutation);
        public abstract void save(string fileId = null);
        public abstract void load(string fileId = null);
        public abstract void clear(string fileId = null);
        public abstract void saveStagedChanges();
        public abstract void clearStagedChanges();
        public abstract void onBeforeClose();
        public abstract void onClose();
        public abstract void useDefaultFile();
        public abstract bool fileExists(string fileId);
        public abstract string getFileName(string fildId);
        public abstract string getTempFileId(string fileId);
        public abstract string getMostRecentFileId();
        public abstract bool anyFileExists();
        public abstract bool anyTempFileExists();
        public abstract bool tempFileExists(string fileId = null);
        public abstract string getJson();
        public abstract T GetValue<T>(string name);
        public abstract void SetValue<T>(string name, T value);
        public abstract FileHandle handle(string fileId, bool loadFile = false);

        public event Action stateUpdated;

        protected void issueStateUpdated()
        {
            stateUpdated?.Invoke();
        }
    }
}